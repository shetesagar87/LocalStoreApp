using CleanMvcApp.Models.Entities;
using CleanMvcApp.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace CleanMvcApp.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CategoryService> _logger;
        private const string CACHE_KEY_ALL_CATEGORIES = "all_categories";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

        public CategoryService(
            IUnitOfWork unitOfWork,
            IMemoryCache cache,
            ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            try
            {
                category.CreatedAt = DateTime.UtcNow;
                category.UpdatedAt = DateTime.UtcNow;

                var createdCategory = await _unitOfWork.Categories.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();

                // Invalidate cache
                _cache.Remove(CACHE_KEY_ALL_CATEGORIES);

                _logger.LogInformation("Category {CategoryName} created with ID {CategoryId}", 
                    category.CategoryName, createdCategory.CategoryId);

                return createdCategory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category {CategoryName}", category.CategoryName);
                throw;
            }
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            return await _unitOfWork.Categories.GetByIdAsync(categoryId);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            // Try to get from cache
            if (!_cache.TryGetValue(CACHE_KEY_ALL_CATEGORIES, out IEnumerable<Category>? categories))
            {
                // Not in cache, fetch from database
                categories = await _unitOfWork.Categories.GetAllAsync();
                
                // Store in cache
                _cache.Set(CACHE_KEY_ALL_CATEGORIES, categories, CacheDuration);
                _logger.LogInformation("Categories loaded from database and cached");
            }
            else
            {
                _logger.LogInformation("Categories loaded from cache");
            }

            return categories ?? Enumerable.Empty<Category>();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            try
            {
                category.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Categories.UpdateAsync(category);
                await _unitOfWork.SaveChangesAsync();

                // Invalidate cache
                _cache.Remove(CACHE_KEY_ALL_CATEGORIES);

                _logger.LogInformation("Category {CategoryId} updated", category.CategoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {CategoryId}", category.CategoryId);
                throw;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
                if (category == null)
                {
                    _logger.LogWarning("Category {CategoryId} not found for deletion", categoryId);
                    return false;
                }

                await _unitOfWork.Categories.DeleteAsync(category);
                await _unitOfWork.SaveChangesAsync();

                // Invalidate cache
                _cache.Remove(CACHE_KEY_ALL_CATEGORIES);

                _logger.LogInformation("Category {CategoryId} deleted", categoryId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {CategoryId}", categoryId);
                throw;
            }
        }
    }
}
