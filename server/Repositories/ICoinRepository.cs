using server.Models;

namespace server.Repositories
{
    public interface ICoinRepository
    {
        Task<List<Coin>> GetAvailableCoinsAsync();
        Task<Coin?> GetCoinByDenominationAsync(int denomination);
        Task<Coin?> GetCoinByIdAsync(int id);
        Task UpdateCoinAsync(Coin coin);
        Task AddCoinAsync(Coin coin);
        Task DeleteCoinAsync(int id);
        Task<List<Coin>> GetAllCoinsAsync();
        Task<bool> CoinExistsAsync(int denomination);
    }
}