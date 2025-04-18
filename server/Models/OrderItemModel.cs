using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
    [Table("order_items")]
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Order")]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Required]
        [Column("drink_name", TypeName = "varchar(100)")]
        public string DrinkName { get; set; }

        [Required]
        [Column("brand_name", TypeName = "varchar(100)")]
        public string BrandName { get; set; }

        [Required]
        [Column("price_at_purchase", TypeName = "decimal(10,2)")]
        public decimal PriceAtPurchase { get; set; }

        [Required]
        [Column("quantity")]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Column("total_price", TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        public Order Order { get; set; }

        [Column("image_url_at_purchase", TypeName = "varchar(255)")]
        public string? ImageUrlAtPurchase { get; set; }
    }
}