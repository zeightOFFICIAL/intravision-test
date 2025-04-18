using server.Models;

namespace server.Repositories
{
    public interface IDrinkRepository
    {
        Task<Drink?> GetDrinkByIdAsync(int id);
        Task<Drink?> GetDrinkByNameAndBrandAsync(string name, string brandName);
        Task<IEnumerable<Drink>> GetAllDrinksAsync();
        Task<IEnumerable<Drink>> GetAvailableDrinksAsync();
        Task AddDrinkAsync(Drink drink);
        Task UpdateDrinkAsync(Drink drink);
        Task DeleteDrinkAsync(int id);
        Task<bool> DrinkExistsAsync(int id);
    }
}