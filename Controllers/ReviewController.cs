using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CleanMvcApp.Services;
using System.Security.Claims;

namespace CleanMvcApp.Controllers
{
    [Authorize(Roles = "Customer")]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IOrderService _orderService;
        private readonly IStoreService _storeService;
        private readonly IProductService _productService;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(
            IReviewService reviewService,
            IOrderService orderService,
            IStoreService storeService,
            IProductService productService,
            ILogger<ReviewController> logger)
        {
            _reviewService = reviewService;
            _orderService = orderService;
            _storeService = storeService;
            _productService = productService;
            _logger = logger;
        }

        // GET: Review/CreateStoreReview?orderId=5
        public async Task<IActionResult> CreateStoreReview(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Verify customer can review this order
            if (!await _reviewService.CanCustomerReviewOrderAsync(userId, orderId))
            {
                TempData["Error"] = "You can only review delivered orders.";
                return RedirectToAction("Details", "Order", new { id = orderId });
            }

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("MyOrders", "Order");
            }

            ViewBag.Order = order;
            ViewBag.Store = order.Store;
            return View();
        }

        // POST: Review/CreateStoreReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStoreReview(int orderId, int storeId, int rating, string comment)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                await _reviewService.CreateStoreReviewAsync(userId, storeId, orderId, rating, comment);
                TempData["Success"] = "Store review submitted successfully!";
                return RedirectToAction("Details", "Order", new { id = orderId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("CreateStoreReview", new { orderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating store review");
                TempData["Error"] = "An error occurred while submitting your review.";
                return RedirectToAction("CreateStoreReview", new { orderId });
            }
        }

        // GET: Review/CreateProductReview?orderId=5&productId=10
        public async Task<IActionResult> CreateProductReview(int orderId, int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Verify customer can review this order
            if (!await _reviewService.CanCustomerReviewOrderAsync(userId, orderId))
            {
                TempData["Error"] = "You can only review delivered orders.";
                return RedirectToAction("Details", "Order", new { id = orderId });
            }

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("MyOrders", "Order");
            }

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction("Details", "Order", new { id = orderId });
            }

            ViewBag.Order = order;
            ViewBag.Product = product;
            return View();
        }

        // POST: Review/CreateProductReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProductReview(int orderId, int productId, int rating, string comment)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                await _reviewService.CreateProductReviewAsync(userId, productId, orderId, rating, comment);
                TempData["Success"] = "Product review submitted successfully!";
                return RedirectToAction("Details", "Order", new { id = orderId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("CreateProductReview", new { orderId, productId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product review");
                TempData["Error"] = "An error occurred while submitting your review.";
                return RedirectToAction("CreateProductReview", new { orderId, productId });
            }
        }

        // GET: Review/StoreReviews/5
        [AllowAnonymous]
        public async Task<IActionResult> StoreReviews(int id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store == null)
            {
                TempData["Error"] = "Store not found.";
                return RedirectToAction("Index", "Home");
            }

            var reviews = await _reviewService.GetStoreReviewsAsync(id);
            ViewBag.Store = store;
            return View(reviews);
        }

        // GET: Review/ProductReviews/10
        [AllowAnonymous]
        public async Task<IActionResult> ProductReviews(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction("Search", "Browse");
            }

            var reviews = await _reviewService.GetProductReviewsAsync(id);
            ViewBag.Product = product;
            return View(reviews);
        }
    }
}
