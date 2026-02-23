using CleanMvcApp.Models.Entities;
using CleanMvcApp.Repositories;

namespace CleanMvcApp.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
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
            return await _unitOfWork.Categories.GetAllAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            try
            {
                category.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Categories.UpdateAsync(category);
                await _unitOfWork.SaveChangesAsync();

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
