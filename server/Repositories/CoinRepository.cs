using server.Context;
using Microsoft.EntityFrameworkCore;
using server.Models;

namespace server.Repositories
{
    public class CoinRepository(
        VendingMachineContext context,
        ILogger<CoinRepository> logger) : ICoinRepository
    {
        private readonly VendingMachineContext _context = context;
        private readonly ILogger<CoinRepository> _logger = logger;

        public async Task<List<Coin>> GetAvailableCoinsAsync()
        {
            try
            {
                return await _context.Coins
                    .Where(c => !c.IsBlocked && c.Amount > 0)
                    .OrderBy(c => c.Denomination)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available coins");
                throw;
            }
        }

        public async Task<Coin?> GetCoinByDenominationAsync(int denomination)
        {
            try
            {
                return await _context.Coins
                    .FirstOrDefaultAsync(c => c.Denomination == denomination);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting coin by denomination: {Denomination}", denomination);
                throw;
            }
        }

        public async Task<Coin?> GetCoinByIdAsync(int id)
        {
            try
            {
                return await _context.Coins.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting coin by ID: {Id}", id);
                throw;
            }
        }

        public async Task UpdateCoinAsync(Coin coin)
        {
            try
            {
                _context.Entry(coin).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await CoinExistsAsync(coin.Denomination))
                {
                    _logger.LogError("Coin not found during update: {Denomination}", coin.Denomination);
                    throw new KeyNotFoundException($"Coin with denomination {coin.Denomination} not found");
                }
                else
                {
                    _logger.LogError(ex, "Concurrency error updating coin: {Denomination}", coin.Denomination);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating coin: {Denomination}", coin.Denomination);
                throw;
            }
        }

        public async Task AddCoinAsync(Coin coin)
        {
            try
            {
                _context.Coins.Add(coin);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new coin: {Denomination}", coin.Denomination);
                throw;
            }
        }

        public async Task DeleteCoinAsync(int id)
        {
            try
            {
                var coin = await _context.Coins.FindAsync(id);
                if (coin == null)
                {
                    throw new KeyNotFoundException($"Coin with ID {id} not found");
                }

                _context.Coins.Remove(coin);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting coin: {Id}", id);
                throw;
            }
        }

        public async Task<List<Coin>> GetAllCoinsAsync()
        {
            try
            {
                return await _context.Coins
                    .OrderBy(c => c.Denomination)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all coins");
                throw;
            }
        }

        public async Task<bool> CoinExistsAsync(int denomination)
        {
            try
            {
                return await _context.Coins
                    .AnyAsync(c => c.Denomination == denomination);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if coin exists: {Denomination}", denomination);
                throw;
            }
        }
    }
}
