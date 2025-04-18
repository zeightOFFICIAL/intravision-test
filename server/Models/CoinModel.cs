using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
    [Table("coins")]
    public class Coin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("denomination", TypeName = "integer")]
        public int Denomination { get; set; }

        [Required]
        [Column("amount_available")]
        [Range(0, int.MaxValue, ErrorMessage = "Amount cannot be negative")]
        public int Amount { get; set; }

        [Required]
        [Column("is_blocked")]
        public bool IsBlocked { get; set; } = false;

        [Required]
        [Column("coin_type", TypeName = "varchar(20)")]
        public string? CoinType { get; set; }

        [NotMapped]
        public string DisplayName => $"{Denomination} RUB";

        [NotMapped]
        public bool CanUse => !IsBlocked && Amount > 0;
    }
}