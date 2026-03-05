using CleanMvcApp.Models.Entities;
using CleanMvcApp.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace CleanMvcApp.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ProductService> _logger;
        private const int LOW_STOCK_THRESHOLD = 10;
        private const string CACHE_KEY_PRODUCT_PREFIX = "product_";
        private const string CACHE_KEY_STORE_PRODUCTS_PREFIX = "store_products_";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

        public ProductService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IMemoryCache cache,
            ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            try
            {
                // Check SKU uniqueness within store
                var existingProduct = await _unitOfWork.Products.FindAsync(p => 
                    p.StoreId == product.StoreId && p.SKU == product.SKU);
                
                if (existingProduct.Any())
                {
                    throw new InvalidOperationException($"Product with SKU {product.SKU} already exists in this store.");
                }

                product.IsDeleted = false;
                product.CreatedAt = DateTime.UtcNow;
                product.UpdatedAt = DateTime.UtcNow;

                var createdProduct = await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();

                // Invalidate cache
                _cache.Remove($"{CACHE_KEY_STORE_PRODUCTS_PREFIX}{product.StoreId}");

                _logger.LogInformation("Product {ProductName} created with ID {ProductId} for store {StoreId}", 
                    product.ProductName, createdProduct.ProductId, product.StoreId);

                return createdProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product {ProductName}", product.ProductName);
                throw;
            }
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            var products = await _unitOfWork.Products.FindAsync(p => p.ProductId == productId && !p.IsDeleted);
            return products.FirstOrDefault();
        }

        public async Task<IEnumerable<Product>> GetProductsByStoreAsync(int storeId)
        {
            return await _unitOfWork.Products.FindAsync(p => p.StoreId == storeId && !p.IsDeleted);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _unitOfWork.Products.FindAsync(p => !p.IsDeleted);
        }

        public async Task UpdateProductAsync(Product product)
        {
            try
            {
                product.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Products.UpdateAsync(product);
                await _unitOfWork.SaveChangesAsync();

                // Invalidate cache
                _cache.Remove($"{CACHE_KEY_PRODUCT_PREFIX}{product.ProductId}");
                _cache.Remove($"{CACHE_KEY_STORE_PRODUCTS_PREFIX}{product.StoreId}");

                _logger.LogInformation("Product {ProductId} updated", product.ProductId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", product.ProductId);
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning("Product {ProductId} not found for deletion", productId);
                    return false;
                }

                // Soft delete
                product.IsDeleted = true;
                product.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Products.UpdateAsync(product);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Product {ProductId} soft deleted", productId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", productId);
                throw;
            }
        }

        public async Task<bool> UpdateStockAsync(int productId, int quantity)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning("Product {ProductId} not found for stock update", productId);
                    return false;
                }

                product.StockQuantity = quantity;
                product.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Products.UpdateAsync(product);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Product {ProductId} stock updated to {Quantity}", productId, quantity);
                
                // Check for low stock and send alert
                if (quantity <= LOW_STOCK_THRESHOLD && quantity > 0)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _notificationService.SendLowStockAlertAsync(product);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send low stock alert for Product {ProductId}", productId);
                        }
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for product {ProductId}", productId);
                throw;
            }
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(
            string? searchTerm, 
            int? categoryId, 
            decimal? minPrice, 
            decimal? maxPrice, 
            int? storeId, 
            decimal? minRating)
        {
            var products = await _unitOfWork.Products.FindAsync(p => !p.IsDeleted);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                products = products.Where(p => 
                    p.ProductName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (p.Description != null && p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            if (storeId.HasValue)
            {
                products = products.Where(p => p.StoreId == storeId.Value);
            }

            if (minRating.HasValue)
            {
                products = products.Where(p => p.AverageRating >= minRating.Value);
            }

            // Filter out products with zero stock
            products = products.Where(p => p.StockQuantity > 0);

            _logger.LogInformation("Product search returned {Count} results", products.Count());
            return products.ToList();
        }
    }
}
