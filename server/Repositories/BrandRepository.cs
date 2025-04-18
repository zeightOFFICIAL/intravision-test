using Microsoft.EntityFrameworkCore;
using server.Context;
using server.Models;

namespace server.Repositories
{
    public class BrandRepository(
        VendingMachineContext context,
        ILogger<BrandRepository> logger) : IBrandRepository
    {
        private readonly VendingMachineContext _context = context;
        private readonly ILogger<BrandRepository> _logger = logger;

        public async Task<Brand?> GetBrandByIdAsync(int id)
        {
            try
            {
                return await _context.Brands
                    .Include(b => b.Drinks)
                    .FirstOrDefaultAsync(b => b.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting brand by ID: {BrandId}", id);
                throw;
            }
        }

        public async Task<Brand?> GetBrandByNameAsync(string name)
        {
            try
            {
                return await _context.Brands
                    .FirstOrDefaultAsync(b => b.Name == name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting brand by name: {BrandName}", name);
                throw;
            }
        }

        public async Task<IEnumerable<Brand>> GetAllBrandsAsync()
        {
            try
            {
                return await _context.Brands
                    .Include(b => b.Drinks)
                    .OrderBy(b => b.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all brands");
                throw;
            }
        }

        public async Task AddBrandAsync(Brand brand)
        {
            try
            {
                await _context.Brands.AddAsync(brand);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new brand: {BrandName}", brand.Name);
                throw;
            }
        }

        public async Task UpdateBrandAsync(Brand brand)
        {
            try
            {
                _context.Entry(brand).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await BrandExistsAsync(brand.Id))
                {
                    _logger.LogError("Brand not found during update: {BrandId}", brand.Id);
                    throw new KeyNotFoundException($"Brand with ID {brand.Id} not found");
                }
                else
                {
                    _logger.LogError(ex, "Concurrency error updating brand: {BrandId}", brand.Id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating brand: {BrandId}", brand.Id);
                throw;
            }
        }

        public async Task DeleteBrandAsync(int id)
        {
            try
            {
                var brand = await _context.Brands.FindAsync(id);
                if (brand == null)
                {
                    throw new KeyNotFoundException($"Brand with ID {id} not found");
                }

                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting brand: {BrandId}", id);
                throw;
            }
        }

        public async Task<bool> BrandExistsAsync(int id)
        {
            return await _context.Brands.AnyAsync(e => e.Id == id);
        }
    }
}