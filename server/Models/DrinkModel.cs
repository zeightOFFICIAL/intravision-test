using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
    [Table("drinks")]
    public class Drink
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name", TypeName = "varchar(100)")]
        public required string Name { get; set; }

        [Required]
        [Column("price", TypeName = "decimal(10,2)")]
        [Range(0.01, 1000.00, ErrorMessage = "Price must be between 0.01 and 1000.00")]
        public decimal Price { get; set; }

        [Required]
        [Column("amount")]
        [Range(0, int.MaxValue, ErrorMessage = "Amount cannot be negative")]
        public int Amount { get; set; }

        [ForeignKey("Brand")]
        [Column("brand_id")]
        public int BrandId { get; set; }

        public Brand Brand { get; set; }

        [Column("image_url", TypeName = "varchar(255)")]
        public string? ImageUrl { get; set; }

        [Column("description", TypeName = "text")]
        public string? Description { get; set; }

        [NotMapped]
        public bool IsAvailable => Amount > 0;
    }
}