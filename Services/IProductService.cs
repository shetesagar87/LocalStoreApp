using CleanMvcApp.Models.Entities;

namespace CleanMvcApp.Services
{
    public interface IProductService
    {
        Task<Product> CreateProductAsync(Product product);
        Task<Product?> GetProductByIdAsync(int productId);
        Task<IEnumerable<Product>> GetProductsByStoreAsync(int storeId);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int productId);
        Task<bool> UpdateStockAsync(int productId, int quantity);
        Task<IEnumerable<Product>> SearchProductsAsync(string? searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice, int? storeId, decimal? minRating);
    }
}
