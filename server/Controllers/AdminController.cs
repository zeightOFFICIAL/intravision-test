using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using server.Models;
using server.Models.DTO;
using server.Repositories;
using server.Services;

namespace server.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    public class OrdersController(
        IOrderService orderService,
        ILogger<OrdersController> logger) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;
        private readonly ILogger<OrdersController> _logger = logger;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);

                if (order == null)
                {
                    return NotFound();
                }

                return Ok(order);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }

    [ApiController]
    [Route("api/admin/[controller]")]
    public class CoinController(
        ICoinRepository coinRepository,
        ILogger<CoinController> logger) : ControllerBase
    {
        private readonly ICoinRepository _coinRepository = coinRepository;
        private readonly ILogger<CoinController> _logger = logger;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coin>>> GetAllCoins()
        {
            try
            {
                var coins = await _coinRepository.GetAllCoinsAsync();
                return Ok(coins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all coins");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<Coin>>> GetAvailableCoins()
        {
            try
            {
                var coins = await _coinRepository.GetAvailableCoinsAsync();
                return Ok(coins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available coins");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{denomination}")]
        public async Task<ActionResult<Coin>> GetCoinByDenomination(int denomination)
        {
            try
            {
                var coin = await _coinRepository.GetCoinByDenominationAsync(denomination);
                if (coin == null)
                {
                    return NotFound();
                }
                return Ok(coin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting coin by denomination: {Denomination}", denomination);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Coin>> AddCoin([FromBody] CoinCreateDto coinDto)
        {
            try
            {
                var existingCoin = await _coinRepository.GetCoinByDenominationAsync(coinDto.Denomination);
                if (existingCoin != null)
                {
                    return Conflict("Coin with this denomination already exists");
                }

                var coin = new Coin
                {
                    Denomination = coinDto.Denomination,
                    Amount = coinDto.Amount,
                    IsBlocked = coinDto.IsBlocked,
                    CoinType = coinDto.CoinType
                };

                await _coinRepository.AddCoinAsync(coin);
                return CreatedAtAction(nameof(GetCoinByDenomination),
                    new { denomination = coin.Denomination }, coin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new coin");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{denomination}")]
        public async Task<IActionResult> UpdateCoin(int denomination, [FromBody] CoinUpdateDto coinDto)
        {
            try
            {
                var coin = await _coinRepository.GetCoinByDenominationAsync(denomination);
                if (coin == null)
                {
                    return NotFound();
                }

                coin.Amount = coinDto.Amount;
                coin.IsBlocked = coinDto.IsBlocked;
                coin.CoinType = coinDto.CoinType;

                await _coinRepository.UpdateCoinAsync(coin);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating coin: {Denomination}", denomination);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("{denomination}/amount")]
        public async Task<IActionResult> SetCoinAmount(int denomination, [FromBody] SetAmountDto amountDto)
        {
            try
            {
                var coin = await _coinRepository.GetCoinByDenominationAsync(denomination);
                if (coin == null)
                {
                    return NotFound();
                }

                coin.Amount = amountDto.Amount;
                await _coinRepository.UpdateCoinAsync(coin);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting coin amount: {Denomination}", denomination);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("{denomination}/block")]
        public async Task<IActionResult> BlockCoin(int denomination, [FromBody] bool isBlocked)
        {
            try
            {
                var coin = await _coinRepository.GetCoinByDenominationAsync(denomination);
                if (coin == null)
                {
                    return NotFound();
                }

                coin.IsBlocked = isBlocked;
                await _coinRepository.UpdateCoinAsync(coin);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blocking/unblocking coin: {Denomination}", denomination);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{denomination}/add")]
        public async Task<IActionResult> AddCoins(int denomination, [FromBody] int quantity)
        {
            try
            {
                var coin = await _coinRepository.GetCoinByDenominationAsync(denomination);
                if (coin == null)
                {
                    return NotFound();
                }

                coin.Amount += quantity;
                await _coinRepository.UpdateCoinAsync(coin);
                return Ok(new { newAmount = coin.Amount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding coins: {Denomination}", denomination);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    [ApiController]
    [Route("api/admin/DrinksController")]
    public class AdminDrinksController(
        IDrinkRepository drinkRepository,
        IBrandRepository brandRepository,
        IMapper mapper,
        ILogger<DrinksController> logger) : ControllerBase
    {
        private readonly IDrinkRepository _drinkRepository = drinkRepository;
        private readonly IBrandRepository _brandRepository = brandRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<DrinksController> _logger = logger;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DrinkDto>>> GetAllDrinks()
        {
            try
            {
                var drinks = await _drinkRepository.GetAllDrinksAsync();
                return Ok(_mapper.Map<IEnumerable<DrinkDto>>(drinks));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all drinks");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<DrinkDto>>> GetAvailableDrinks()
        {
            try
            {
                var drinks = await _drinkRepository.GetAvailableDrinksAsync();
                return Ok(_mapper.Map<IEnumerable<DrinkDto>>(drinks));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available drinks");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DrinkDto>> GetDrinkById(int id)
        {
            try
            {
                var drink = await _drinkRepository.GetDrinkByIdAsync(id);
                if (drink == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<DrinkDto>(drink));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting drink by ID: {DrinkId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<DrinkDto>> CreateDrink([FromBody] DrinkCreateDto drinkDto)
        {
            try
            {
                var brand = await _brandRepository.GetBrandByIdAsync(drinkDto.BrandId);
                if (brand == null)
                {
                    return BadRequest("Invalid brand ID");
                }

                var drink = _mapper.Map<Drink>(drinkDto);
                drink.Brand = brand;

                await _drinkRepository.AddDrinkAsync(drink);

                var createdDrink = _mapper.Map<DrinkDto>(drink);
                return CreatedAtAction(
                    nameof(GetDrinkById),
                    new { id = createdDrink.Id },
                    createdDrink);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new drink");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDrink(int id, [FromBody] DrinkUpdateDto drinkDto)
        {
            try
            {
                if (id != drinkDto.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var existingDrink = await _drinkRepository.GetDrinkByIdAsync(id);
                if (existingDrink == null)
                {
                    return NotFound();
                }

                var brand = await _brandRepository.GetBrandByIdAsync(drinkDto.BrandId);
                if (brand == null)
                {
                    return BadRequest("Invalid brand ID");
                }

                _mapper.Map(drinkDto, existingDrink);
                existingDrink.Brand = brand;

                await _drinkRepository.UpdateDrinkAsync(existingDrink);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating drink: {DrinkId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDrink(int id)
        {
            try
            {
                var drink = await _drinkRepository.GetDrinkByIdAsync(id);
                if (drink == null)
                {
                    return NotFound();
                }

                await _drinkRepository.DeleteDrinkAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting drink: {DrinkId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockDto stockDto)
        {
            try
            {
                var drink = await _drinkRepository.GetDrinkByIdAsync(id);
                if (drink == null)
                {
                    return NotFound();
                }

                drink.Amount = stockDto.Amount;
                await _drinkRepository.UpdateDrinkAsync(drink);
                return Ok(_mapper.Map<DrinkDto>(drink));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating drink stock: {DrinkId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}