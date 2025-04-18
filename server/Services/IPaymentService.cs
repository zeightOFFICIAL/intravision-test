using server.Models;
using server.Models.DTO;

namespace server.Services
{
    public interface IPaymentService
    {
        Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
        Task<CoinStatusResult> GetCoinStatusAsync();
    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public int? OrderId { get; set; }
        public decimal? ChangeAmount { get; set; }
        public List<CoinChange>? ChangeCoins { get; set; }
        public string? Error { get; set; }
        public string? Message { get; set; }
    }

    public class CoinStatusResult
    {
        public List<CoinStatus> Coins { get; set; } = new List<CoinStatus>();
    }

    public class CoinStatus
    {
        public int Denomination { get; set; }
        public int Amount { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public bool CanUse { get; set; }
    }
}