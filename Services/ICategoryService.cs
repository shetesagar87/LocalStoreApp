using CleanMvcApp.Models.Entities;

namespace CleanMvcApp.Services
{
    public interface ICategoryService
    {
        Task<Category> CreateCategoryAsync(Category category);
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int categoryId);
    }
}
