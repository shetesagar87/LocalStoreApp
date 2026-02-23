using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CleanMvcApp.Services;
using CleanMvcApp.Models.Enums;
using System.Security.Claims;

namespace CleanMvcApp.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IStoreService _storeService;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(
            ICartService cartService,
            IOrderService orderService,
            IStoreService storeService,
            ILogger<CheckoutController> logger)
        {
            _cartService = cartService;
            _orderService = orderService;
            _storeService = storeService;
            _logger = logger;
        }

        // GET: Checkout
        public async Task<IActionResult> Index(int storeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Get cart items for this store
            var allCartItems = await _cartService.GetCartAsync(userId);
            var cartItems = allCartItems.Where(c => c.Product.StoreId == storeId).ToList();

            if (!cartItems.Any())
            {
                TempData["Error"] = "No items in cart for this store.";
                return RedirectToAction("Index", "Cart");
            }

            // Get store details
            var store = await _storeService.GetStoreByIdAsync(storeId);
            if (store == null)
            {
                TempData["Error"] = "Store not found.";
                return RedirectToAction("Index", "Home");
            }

            // Calculate totals
            decimal subtotal = 0;
            foreach (var item in cartItems)
            {
                var product = item.Product;
                var itemPrice = product.Price * (1 - product.DiscountPercentage / 100);
                subtotal += itemPrice * item.Quantity;
            }

            ViewBag.Store = store;
            ViewBag.CartItems = cartItems;
            ViewBag.Subtotal = subtotal;
            ViewBag.DeliveryCharge = store.DeliveryCharge;
            ViewBag.Total = subtotal + store.DeliveryCharge;

            // Get user's address for default delivery address
            var userAddress = User.FindFirst("Address")?.Value ?? "";
            ViewBag.UserAddress = userAddress;

            return View();
        }

        // POST: Checkout/PlaceOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(int storeId, string deliveryAddress, string paymentMethod)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                if (string.IsNullOrWhiteSpace(deliveryAddress))
                {
                    TempData["Error"] = "Please provide a delivery address.";
                    return RedirectToAction("Index", new { storeId });
                }

                // Parse payment method
                if (!Enum.TryParse<PaymentMethod>(paymentMethod, out var paymentMethodEnum))
                {
                    TempData["Error"] = "Invalid payment method.";
                    return RedirectToAction("Index", new { storeId });
                }

                // Create order
                var order = await _orderService.CreateOrderAsync(userId, storeId, deliveryAddress, paymentMethodEnum);

                TempData["Success"] = $"Order placed successfully! Order Number: {order.OrderNumber}";

                // Redirect based on payment method
                if (paymentMethodEnum == PaymentMethod.CashOnDelivery)
                {
                    return RedirectToAction("OrderConfirmation", new { orderNumber = order.OrderNumber });
                }
                else
                {
                    // Redirect to payment gateway (to be implemented)
                    return RedirectToAction("ProcessPayment", "Payment", new { orderId = order.OrderId });
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", new { storeId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing order");
                TempData["Error"] = "An error occurred while placing your order. Please try again.";
                return RedirectToAction("Index", new { storeId });
            }
        }

        // GET: Checkout/OrderConfirmation
        public async Task<IActionResult> OrderConfirmation(string orderNumber)
        {
            var order = await _orderService.GetOrderByNumberAsync(orderNumber);
            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Index", "Home");
            }

            // Verify user owns this order
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (order.CustomerId != userId)
            {
                return Forbid();
            }

            return View(order);
        }
    }
}
