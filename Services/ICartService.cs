using CleanMvcApp.Models.Entities;

namespace CleanMvcApp.Services
{
    public interface ICartService
    {
        Task<CartItem> AddToCartAsync(string userId, int productId, int quantity);
        Task<bool> UpdateCartItemAsync(int cartItemId, int quantity);
        Task<bool> RemoveFromCartAsync(int cartItemId);
        Task<IEnumerable<CartItem>> GetCartAsync(string userId);
        Task<decimal> GetCartSubtotalAsync(string userId);
        Task<bool> ClearCartAsync(string userId);
        Task<int> GetCartItemCountAsync(string userId);
    }
}
