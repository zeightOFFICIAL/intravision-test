using Microsoft.EntityFrameworkCore;
using server.Context;
using server.Models;
using server.Repositories;
using server.Models.DTO;

namespace server.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly VendingMachineContext _context;
        private readonly ILogger<PaymentService> _logger;
        private readonly IDrinkRepository _drinkRepository;
        private readonly ICoinRepository _coinRepository;
        private readonly IOrderRepository _orderRepository;

        public PaymentService(
            VendingMachineContext context,
            ILogger<PaymentService> logger,
            IDrinkRepository drinkRepository,
            ICoinRepository coinRepository,
            IOrderRepository orderRepository)
        {
            _context = context;
            _logger = logger;
            _drinkRepository = drinkRepository;
            _coinRepository = coinRepository;
            _orderRepository = orderRepository;
        }

        public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Validate request
                var validationResult = ValidatePaymentRequest(request);
                if (!validationResult.Success)
                {
                    return validationResult;
                }

                // 2. Calculate totals
                var totalInserted = request.Coins.Sum(c => c.Denomination * c.Quantity);
                var orderTotal = request.Items.Sum(i => i.PriceAtPurchase * i.Quantity);

                if (totalInserted < orderTotal)
                {
                    return new PaymentResult
                    {
                        Success = false,
                        Error = "insufficient_funds",
                        Message = "Insufficient funds for payment"
                    };
                }

                // 3. Create and save order
                var order = await CreateOrderAsync(request, orderTotal);

                // 4. Process change
                var changeResult = await ProcessChangeAsync(totalInserted, orderTotal);
                if (!changeResult.Success)
                {
                    await UpdateFailedOrderAsync(order, changeResult.Message);
                    await transaction.CommitAsync();
                    return changeResult;
                }

                // 5. Update drink quantities
                var drinkUpdateResult = await UpdateDrinkQuantitiesAsync(request.Items);
                if (!drinkUpdateResult.Success)
                {
                    await UpdateFailedOrderAsync(order, drinkUpdateResult.Message);
                    await transaction.CommitAsync();
                    return drinkUpdateResult;
                }

                // 6. Update coin inventory
                await UpdateCoinInventoryAsync(request.Coins);

                // 7. Finalize successful order
                await FinalizeSuccessfulOrderAsync(order, totalInserted, changeResult.ChangeAmount);

                await transaction.CommitAsync();

                return new PaymentResult
                {
                    Success = true,
                    OrderId = order.Id,
                    ChangeAmount = changeResult.ChangeAmount,
                    ChangeCoins = changeResult.ChangeCoins
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Payment processing failed");
                return new PaymentResult
                {
                    Success = false,
                    Error = "server_error",
                    Message = ex.Message
                };
            }
        }

        public async Task<CoinStatusResult> GetCoinStatusAsync()
        {
            var coins = await _coinRepository.GetAvailableCoinsAsync();
            return new CoinStatusResult
            {
                Coins = coins.Select(c => new CoinStatus
                {
                    Denomination = c.Denomination,
                    Amount = c.Amount,
                    DisplayName = c.DisplayName,
                    CanUse = c.CanUse
                }).ToList()
            };
        }

        private PaymentResult ValidatePaymentRequest(PaymentRequest request)
        {
            if (request.Items == null || !request.Items.Any())
            {
                return new PaymentResult
                {
                    Success = false,
                    Error = "no_items",
                    Message = "No items in payment request"
                };
            }

            if (request.Coins == null || !request.Coins.Any())
            {
                return new PaymentResult
                {
                    Success = false,
                    Error = "no_coins",
                    Message = "No coins in payment request"
                };
            }

            return new PaymentResult { Success = true };
        }

        private async Task<Order> CreateOrderAsync(PaymentRequest request, decimal orderTotal)
        {
            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                TotalAmount = orderTotal,
                FailureReason = string.Empty,
                OrderItems = request.Items.Select(i => new OrderItem
                {
                    DrinkName = i.DrinkName,
                    BrandName = i.BrandName,
                    PriceAtPurchase = i.PriceAtPurchase,
                    Quantity = i.Quantity,
                    TotalPrice = i.PriceAtPurchase * i.Quantity
                }).ToList()
            };

            await _orderRepository.AddOrderAsync(order);
            return order;
        }

        private async Task<PaymentResult> ProcessChangeAsync(decimal totalInserted, decimal orderTotal)
        {
            var changeAmount = totalInserted - orderTotal;
            var changeCoins = new List<CoinChange>();

            if (changeAmount > 0)
            {
                var availableCoins = await _coinRepository.GetAvailableCoinsAsync();
                var remainingChange = changeAmount;

                foreach (var coin in availableCoins.OrderByDescending(c => c.Denomination))
                {
                    if (remainingChange <= 0) break;

                    var needed = (int)(remainingChange / coin.Denomination);
                    var available = Math.Min(needed, coin.Amount);

                    if (available > 0)
                    {
                        changeCoins.Add(new CoinChange
                        {
                            Denomination = coin.Denomination,
                            Quantity = available
                        });

                        coin.Amount -= available;
                        remainingChange -= coin.Denomination * available;
                    }
                }

                if (remainingChange > 0)
                {
                    return new PaymentResult
                    {
                        Success = false,
                        Error = "insufficient_change",
                        Message = "Cannot provide exact change"
                    };
                }
            }

            return new PaymentResult
            {
                Success = true,
                ChangeAmount = changeAmount,
                ChangeCoins = changeCoins
            };
        }

        private async Task<PaymentResult> UpdateDrinkQuantitiesAsync(List<PaymentItem> items)
        {
            foreach (var item in items)
            {
                var drink = await _drinkRepository.GetDrinkByNameAndBrandAsync(item.DrinkName, item.BrandName);
                if (drink == null || drink.Amount < item.Quantity)
                {
                    return new PaymentResult
                    {
                        Success = false,
                        Error = "invalid_item",
                        Message = $"Drink not available: {item.DrinkName}"
                    };
                }

                drink.Amount -= item.Quantity;
                await _drinkRepository.UpdateDrinkAsync(drink);
            }

            return new PaymentResult { Success = true };
        }

        private async Task UpdateCoinInventoryAsync(List<PaymentCoin> coins)
        {
            foreach (var coin in coins)
            {
                var machineCoin = await _coinRepository.GetCoinByDenominationAsync(coin.Denomination);
                if (machineCoin != null)
                {
                    machineCoin.Amount += coin.Quantity;
                    await _coinRepository.UpdateCoinAsync(machineCoin);
                }
                else
                {
                    await _coinRepository.AddCoinAsync(new Coin
                    {
                        Denomination = coin.Denomination,
                        Amount = coin.Quantity,
                        IsBlocked = false,
                        CoinType = "UserInserted"
                    });
                }
            }
        }

        private async Task FinalizeSuccessfulOrderAsync(Order order, decimal amountPaid, decimal? changeAmount)
        {
            order.Status = "Completed";
            order.AmountPaid = amountPaid;
            order.ChangeAmount = changeAmount ?? 0;
            await _orderRepository.UpdateOrderAsync(order);
        }

        private async Task UpdateFailedOrderAsync(Order order, string failureReason)
        {
            order.Status = "Failed";
            order.FailureReason = failureReason;
            await _orderRepository.UpdateOrderAsync(order);
        }
    }
}