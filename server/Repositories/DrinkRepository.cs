using Microsoft.EntityFrameworkCore;
using server.Context;
using server.Models;

namespace server.Repositories
{
    public class DrinkRepository : IDrinkRepository
    {
        private readonly VendingMachineContext _context;
        private readonly ILogger<DrinkRepository> _logger;

        public DrinkRepository(
            VendingMachineContext context,
            ILogger<DrinkRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Drink?> GetDrinkByIdAsync(int id)
        {
            try
            {
                return await _context.Drinks
                    .Include(d => d.Brand)
                    .FirstOrDefaultAsync(d => d.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting drink by ID: {DrinkId}", id);
                throw;
            }
        }

        public async Task<Drink?> GetDrinkByNameAndBrandAsync(string name, string brandName)
        {
            try
            {
                return await _context.Drinks
                    .Include(d => d.Brand)
                    .FirstOrDefaultAsync(d => d.Name == name && d.Brand.Name == brandName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting drink by name and brand: {Name}, {Brand}", name, brandName);
                throw;
            }
        }

        public async Task<IEnumerable<Drink>> GetAllDrinksAsync()
        {
            try
            {
                return await _context.Drinks
                    .Include(d => d.Brand)
                    .OrderBy(d => d.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all drinks");
                throw;
            }
        }

        public async Task<IEnumerable<Drink>> GetAvailableDrinksAsync()
        {
            try
            {
                return await _context.Drinks
                    .Include(d => d.Brand)
                    .Where(d => d.Amount > 0)
                    .OrderBy(d => d.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available drinks");
                throw;
            }
        }

        public async Task AddDrinkAsync(Drink drink)
        {
            try
            {
                await _context.Drinks.AddAsync(drink);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new drink: {DrinkName}", drink.Name);
                throw;
            }
        }

        public async Task UpdateDrinkAsync(Drink drink)
        {
            try
            {
                _context.Entry(drink).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await DrinkExistsAsync(drink.Id))
                {
                    _logger.LogError("Drink not found during update: {DrinkId}", drink.Id);
                    throw new KeyNotFoundException($"Drink with ID {drink.Id} not found");
                }
                else
                {
                    _logger.LogError(ex, "Concurrency error updating drink: {DrinkId}", drink.Id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating drink: {DrinkId}", drink.Id);
                throw;
            }
        }

        public async Task DeleteDrinkAsync(int id)
        {
            try
            {
                var drink = await _context.Drinks.FindAsync(id);
                if (drink == null)
                {
                    throw new KeyNotFoundException($"Drink with ID {id} not found");
                }

                _context.Drinks.Remove(drink);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting drink: {DrinkId}", id);
                throw;
            }
        }

        public async Task<bool> DrinkExistsAsync(int id)
        {
            try
            {
                return await _context.Drinks.AnyAsync(d => d.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if drink exists: {DrinkId}", id);
                throw;
            }
        }
    }
}