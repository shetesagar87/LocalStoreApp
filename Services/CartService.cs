using CleanMvcApp.Models.Entities;
using CleanMvcApp.Repositories;

namespace CleanMvcApp.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CartService> _logger;

        public CartService(IUnitOfWork unitOfWork, ILogger<CartService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CartItem> AddToCartAsync(string userId, int productId, int quantity)
        {
            try
            {
                // Get product to validate stock
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null)
                {
                    throw new InvalidOperationException("Product not found.");
                }

                if (product.StockQuantity < quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock. Only {product.StockQuantity} items available.");
                }

                // Check if item already exists in cart
                var existingCartItems = await _unitOfWork.CartItems.FindAsync(c => 
                    c.CustomerId == userId && c.ProductId == productId);
                var existingCartItem = existingCartItems.FirstOrDefault();

                if (existingCartItem != null)
                {
                    // Update quantity
                    var newQuantity = existingCartItem.Quantity + quantity;
                    if (product.StockQuantity < newQuantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock. Only {product.StockQuantity} items available.");
                    }

                    existingCartItem.Quantity = newQuantity;
                    existingCartItem.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.CartItems.UpdateAsync(existingCartItem);
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation("Updated cart item {CartItemId} for user {UserId}", existingCartItem.CartItemId, userId);
                    return existingCartItem;
                }
                else
                {
                    // Create new cart item
                    var cartItem = new CartItem
                    {
                        CustomerId = userId,
                        ProductId = productId,
                        Quantity = quantity,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    var createdItem = await _unitOfWork.CartItems.AddAsync(cartItem);
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation("Added product {ProductId} to cart for user {UserId}", productId, userId);
                    return createdItem;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product {ProductId} to cart for user {UserId}", productId, userId);
                throw;
            }
        }

        public async Task<bool> UpdateCartItemAsync(int cartItemId, int quantity)
        {
            try
            {
                var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
                if (cartItem == null)
                {
                    _logger.LogWarning("Cart item {CartItemId} not found", cartItemId);
                    return false;
                }

                // Validate stock
                var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
                if (product == null)
                {
                    throw new InvalidOperationException("Product not found.");
                }

                if (product.StockQuantity < quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock. Only {product.StockQuantity} items available.");
                }

                cartItem.Quantity = quantity;
                cartItem.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.CartItems.UpdateAsync(cartItem);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Updated cart item {CartItemId} quantity to {Quantity}", cartItemId, quantity);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item {CartItemId}", cartItemId);
                throw;
            }
        }

        public async Task<bool> RemoveFromCartAsync(int cartItemId)
        {
            try
            {
                var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
                if (cartItem == null)
                {
                    _logger.LogWarning("Cart item {CartItemId} not found", cartItemId);
                    return false;
                }

                await _unitOfWork.CartItems.DeleteAsync(cartItem);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Removed cart item {CartItemId}", cartItemId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item {CartItemId}", cartItemId);
                throw;
            }
        }

        public async Task<IEnumerable<CartItem>> GetCartAsync(string userId)
        {
            return await _unitOfWork.CartItems.FindAsync(c => c.CustomerId == userId);
        }

        public async Task<decimal> GetCartSubtotalAsync(string userId)
        {
            var cartItems = await GetCartAsync(userId);
            decimal subtotal = 0;
            
            foreach (var item in cartItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    var itemPrice = product.Price * (1 - product.DiscountPercentage / 100);
                    subtotal += itemPrice * item.Quantity;
                }
            }
            
            return subtotal;
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            try
            {
                var cartItems = await GetCartAsync(userId);
                foreach (var item in cartItems)
                {
                    await _unitOfWork.CartItems.DeleteAsync(item);
                }
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Cleared cart for user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for user {UserId}", userId);
                throw;
            }
        }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            var cartItems = await GetCartAsync(userId);
            return cartItems.Sum(c => c.Quantity);
        }
    }
}
