using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using server.Context;

namespace server.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    public class TestController(VendingMachineContext context) : ControllerBase
    {
        private readonly VendingMachineContext _context = context;

        [HttpGet]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var brandsCount = await _context.Brands.CountAsync();
                return Ok($"Connection successful! Brands count: {brandsCount}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Connection failed: {ex.Message}");
            }
        }
    }
}
