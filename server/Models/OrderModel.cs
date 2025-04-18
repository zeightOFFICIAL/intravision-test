using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    [Table("orders")]
    public class Order
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("order_date")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("total_amount", TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Column("amount_paid", TypeName = "decimal(10,2)")]
        public decimal AmountPaid { get; set; }

        [Required]
        [Column("change_amount", TypeName = "decimal(10,2)")]
        public decimal ChangeAmount { get; set; }

        [Required]
        [Column("status", TypeName = "varchar(20)")]
        public string Status { get; set; } = "Pending";

        [Column("failure_reason", TypeName = "varchar(255)")]
        public string FailureReason { get; set; } = string.Empty;

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
