using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CleanMvcApp.Services;
using System.Security.Claims;

namespace CleanMvcApp.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly ILogger<CartController> _logger;

        public CartController(
            ICartService cartService,
            IProductService productService,
            ILogger<CartController> logger)
        {
            _cartService = cartService;
            _productService = productService;
            _logger = logger;
        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _cartService.GetCartAsync(userId);
            var subtotal = await _cartService.GetCartSubtotalAsync(userId);

            ViewBag.Subtotal = subtotal;
            return View(cartItems);
        }

        // POST: Cart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Please login to add items to cart." });
                }

                await _cartService.AddToCartAsync(userId, productId, quantity);
                var itemCount = await _cartService.GetCartItemCountAsync(userId);

                return Json(new { success = true, message = "Item added to cart successfully!", itemCount });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product {ProductId} to cart", productId);
                return Json(new { success = false, message = "An error occurred while adding item to cart." });
            }
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            try
            {
                if (quantity <= 0)
                {
                    return Json(new { success = false, message = "Quantity must be greater than 0." });
                }

                var result = await _cartService.UpdateCartItemAsync(cartItemId, quantity);
                if (!result)
                {
                    return Json(new { success = false, message = "Cart item not found." });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var subtotal = await _cartService.GetCartSubtotalAsync(userId!);

                return Json(new { success = true, message = "Quantity updated successfully!", subtotal });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item {CartItemId}", cartItemId);
                return Json(new { success = false, message = "An error occurred while updating quantity." });
            }
        }

        // POST: Cart/RemoveItem
        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            try
            {
                var result = await _cartService.RemoveFromCartAsync(cartItemId);
                if (!result)
                {
                    return Json(new { success = false, message = "Cart item not found." });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var subtotal = await _cartService.GetCartSubtotalAsync(userId!);
                var itemCount = await _cartService.GetCartItemCountAsync(userId!);

                return Json(new { success = true, message = "Item removed from cart.", subtotal, itemCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item {CartItemId}", cartItemId);
                return Json(new { success = false, message = "An error occurred while removing item." });
            }
        }

        // POST: Cart/Clear
        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not found." });
                }

                await _cartService.ClearCartAsync(userId);
                return Json(new { success = true, message = "Cart cleared successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                return Json(new { success = false, message = "An error occurred while clearing cart." });
            }
        }

        // GET: Cart/GetItemCount
        [HttpGet]
        public async Task<IActionResult> GetItemCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { itemCount = 0 });
            }

            var itemCount = await _cartService.GetCartItemCountAsync(userId);
            return Json(new { itemCount });
        }
    }
}
