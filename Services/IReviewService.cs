using CleanMvcApp.Models.Entities;

namespace CleanMvcApp.Services
{
    public interface IReviewService
    {
        Task<Review> CreateStoreReviewAsync(string customerId, int storeId, int orderId, int rating, string comment);
        Task<Review> CreateProductReviewAsync(string customerId, int productId, int orderId, int rating, string comment);
        Task<bool> DeleteReviewAsync(int reviewId);
        Task<IEnumerable<Review>> GetStoreReviewsAsync(int storeId);
        Task<IEnumerable<Review>> GetProductReviewsAsync(int productId);
        Task<decimal> CalculateStoreRatingAsync(int storeId);
        Task<decimal> CalculateProductRatingAsync(int productId);
        Task<bool> CanCustomerReviewOrderAsync(string customerId, int orderId);
    }
}
