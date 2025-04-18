using Microsoft.AspNetCore.Mvc;

using server.Services;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DrinksController(IDrinkService drinkService) : ControllerBase
    {
        private readonly IDrinkService _drinkService = drinkService;

        [HttpGet]
        public async Task<IActionResult> GetAllDrinks()
        {
            var drinks = await _drinkService.GetAllDrinksForDisplayAsync();
            return Ok(drinks);
        }
    }

}