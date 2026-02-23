using CleanMvcApp.Models.Entities;
using CleanMvcApp.Models.Enums;
using CleanMvcApp.Repositories;

namespace CleanMvcApp.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(
            IUnitOfWork unitOfWork,
            ILogger<ReviewService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Review> CreateStoreReviewAsync(string customerId, int storeId, int orderId, int rating, string comment)
        {
            try
            {
                // Validate rating
                if (rating < 1 || rating > 5)
                {
                    throw new ArgumentException("Rating must be between 1 and 5.");
                }

                // Verify order exists and is delivered
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    throw new InvalidOperationException("Order not found.");
                }

                if (order.CustomerId != customerId)
                {
                    throw new InvalidOperationException("You can only review your own orders.");
                }

                if (order.Status != OrderStatus.Delivered)
                {
                    throw new InvalidOperationException("You can only review delivered orders.");
                }

                if (order.StoreId != storeId)
                {
                    throw new InvalidOperationException("This order is not from the specified store.");
                }

                // Check if review already exists
                var existingReviews = await _unitOfWork.Reviews.FindAsync(r => 
                    r.CustomerId == customerId && r.StoreId == storeId && r.OrderId == orderId);
                
                if (existingReviews.Any())
                {
                    throw new InvalidOperationException("You have already reviewed this store for this order.");
                }

                // Create review
                var review = new Review
                {
                    CustomerId = customerId,
                    StoreId = storeId,
                    OrderId = orderId,
                    Rating = rating,
                    Comment = comment,
                    CreatedAt = DateTime.UtcNow
                };

                var createdReview = await _unitOfWork.Reviews.AddAsync(review);
                await _unitOfWork.SaveChangesAsync();

                // Update store average rating
                await UpdateStoreAverageRatingAsync(storeId);

                _logger.LogInformation("Store review created for store {StoreId} by customer {CustomerId}", storeId, customerId);
                return createdReview;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating store review");
                throw;
            }
        }

        public async Task<Review> CreateProductReviewAsync(string customerId, int productId, int orderId, int rating, string comment)
        {
            try
            {
                // Validate rating
                if (rating < 1 || rating > 5)
                {
                    throw new ArgumentException("Rating must be between 1 and 5.");
                }

                // Verify order exists and is delivered
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    throw new InvalidOperationException("Order not found.");
                }

                if (order.CustomerId != customerId)
                {
                    throw new InvalidOperationException("You can only review your own orders.");
                }

                if (order.Status != OrderStatus.Delivered)
                {
                    throw new InvalidOperationException("You can only review delivered orders.");
                }

                // Verify product was in the order
                var orderItems = await _unitOfWork.OrderItems.FindAsync(oi => oi.OrderId == orderId);
                if (!orderItems.Any(oi => oi.ProductId == productId))
                {
                    throw new InvalidOperationException("This product was not in the specified order.");
                }

                // Check if review already exists
                var existingReviews = await _unitOfWork.Reviews.FindAsync(r => 
                    r.CustomerId == customerId && r.ProductId == productId && r.OrderId == orderId);
                
                if (existingReviews.Any())
                {
                    throw new InvalidOperationException("You have already reviewed this product for this order.");
                }

                // Create review
                var review = new Review
                {
                    CustomerId = customerId,
                    ProductId = productId,
                    OrderId = orderId,
                    Rating = rating,
                    Comment = comment,
                    CreatedAt = DateTime.UtcNow
                };

                var createdReview = await _unitOfWork.Reviews.AddAsync(review);
                await _unitOfWork.SaveChangesAsync();

                // Update product average rating
                await UpdateProductAverageRatingAsync(productId);

                _logger.LogInformation("Product review created for product {ProductId} by customer {CustomerId}", productId, customerId);
                return createdReview;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product review");
                throw;
            }
        }

        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            try
            {
                var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
                if (review == null)
                {
                    return false;
                }

                var storeId = review.StoreId;
                var productId = review.ProductId;

                await _unitOfWork.Reviews.DeleteAsync(review);
                await _unitOfWork.SaveChangesAsync();

                // Update average ratings
                if (storeId.HasValue)
                {
                    await UpdateStoreAverageRatingAsync(storeId.Value);
                }

                if (productId.HasValue)
                {
                    await UpdateProductAverageRatingAsync(productId.Value);
                }

                _logger.LogInformation("Review {ReviewId} deleted", reviewId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review {ReviewId}", reviewId);
                throw;
            }
        }

        public async Task<IEnumerable<Review>> GetStoreReviewsAsync(int storeId)
        {
            var reviews = await _unitOfWork.Reviews.FindAsync(r => r.StoreId == storeId);
            return reviews.OrderByDescending(r => r.CreatedAt);
        }

        public async Task<IEnumerable<Review>> GetProductReviewsAsync(int productId)
        {
            var reviews = await _unitOfWork.Reviews.FindAsync(r => r.ProductId == productId);
            return reviews.OrderByDescending(r => r.CreatedAt);
        }

        public async Task<decimal> CalculateStoreRatingAsync(int storeId)
        {
            var reviews = await _unitOfWork.Reviews.FindAsync(r => r.StoreId == storeId);
            if (!reviews.Any())
            {
                return 0;
            }

            return (decimal)reviews.Average(r => r.Rating);
        }

        public async Task<decimal> CalculateProductRatingAsync(int productId)
        {
            var reviews = await _unitOfWork.Reviews.FindAsync(r => r.ProductId == productId);
            if (!reviews.Any())
            {
                return 0;
            }

            return (decimal)reviews.Average(r => r.Rating);
        }

        public async Task<bool> CanCustomerReviewOrderAsync(string customerId, int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
            {
                return false;
            }

            return order.CustomerId == customerId && order.Status == OrderStatus.Delivered;
        }

        private async Task UpdateStoreAverageRatingAsync(int storeId)
        {
            var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
            if (store != null)
            {
                store.AverageRating = await CalculateStoreRatingAsync(storeId);
                store.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Stores.UpdateAsync(store);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private async Task UpdateProductAverageRatingAsync(int productId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product != null)
            {
                product.AverageRating = await CalculateProductRatingAsync(productId);
                product.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Products.UpdateAsync(product);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
