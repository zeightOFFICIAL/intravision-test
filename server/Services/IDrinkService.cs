using server.Models.DTO;

namespace server.Services
{
    public interface IDrinkService
    {
        Task<IEnumerable<DrinkDto>> GetAllDrinksForDisplayAsync();
    }
}
