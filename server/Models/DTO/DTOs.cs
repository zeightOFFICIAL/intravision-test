using System.ComponentModel.DataAnnotations;

namespace server.Models.DTO
{
    public class DrinkDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public BrandDto Brand { get; set; }
    }

    public class BrandDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class DrinkCreateDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public int BrandId { get; set; }
    }

    public class DrinkUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public int BrandId { get; set; }
    }

    public class UpdateStockDto
    {
        public int Amount { get; set; }
    }

    public class BrandCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }

    public class BrandUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }

    public class CoinDto
    {
        public int Denomination { get; set; }
        public int AmountAvailable { get; set; }
        public string DisplayName { get; set; }
        public bool CanUse { get; set; }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal ChangeAmount { get; set; }
        public string Status { get; set; }
        public string FailureReason { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }

    public class OrderItemDto
    {
        public string DrinkName { get; set; }
        public string BrandName { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CoinCreateDto
    {
        public int Denomination { get; set; }
        public int Amount { get; set; }
        public bool IsBlocked { get; set; }
        public string CoinType { get; set; }
    }

    public class CoinUpdateDto
    {
        public int Amount { get; set; }
        public bool IsBlocked { get; set; }
        public string CoinType { get; set; }
    }

    public class SetAmountDto
    {
        public int Amount { get; set; }
    }

    public class PaymentRequest
    {
        public int OrderId { get; set; }  
        public List<PaymentCoin> Coins { get; set; }
        public List<PaymentItem> Items { get; set; } 
    }

    public class PaymentItem
    {
        public string DrinkName { get; set; }
        public string BrandName { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public int Quantity { get; set; }
    }

    public class PaymentCoin
    {
        public int Denomination { get; set; }
        public int Quantity { get; set; }
    }

    public class CoinChange
    {
        public int Denomination { get; set; }
        public int Quantity { get; set; }
    }
}
