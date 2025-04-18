using server.Models;

namespace server.Repositories
{
    public interface IBrandRepository
    {
        Task<Brand?> GetBrandByIdAsync(int id);
        Task<Brand?> GetBrandByNameAsync(string name);
        Task<IEnumerable<Brand>> GetAllBrandsAsync();
        Task AddBrandAsync(Brand brand);
        Task UpdateBrandAsync(Brand brand);
        Task DeleteBrandAsync(int id);
        Task<bool> BrandExistsAsync(int id);
    }
}