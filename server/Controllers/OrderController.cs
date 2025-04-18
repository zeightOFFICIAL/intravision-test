using Microsoft.AspNetCore.Mvc;
using server.Models.DTO;
using server.Services;

namespace server.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController(
        IPaymentService paymentService,
        ILogger<PaymentsController> logger) : ControllerBase
    {
        private readonly IPaymentService _paymentService = paymentService;
        private readonly ILogger<PaymentsController> _logger = logger;

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            var result = await _paymentService.ProcessPaymentAsync(request);

            if (!result.Success)
            {
                _logger.LogError("Payment failed: {Error} - {Message}", result.Error, result.Message);
                return BadRequest(new { result.Error, result.Message });
            }

            return Ok(new
            {
                success = true,
                orderId = result.OrderId,
                change = new
                {
                    amount = result.ChangeAmount,
                    coins = result.ChangeCoins
                }
            });
        }

        [HttpGet("coin-status")]
        public async Task<IActionResult> GetCoinStatus()
        {
            var result = await _paymentService.GetCoinStatusAsync();
            return Ok(result.Coins);
        }
    }
}
