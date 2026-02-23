using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CleanMvcApp.Services;
using System.Security.Claims;

namespace CleanMvcApp.Controllers
{
    [Authorize(Roles = "Customer")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(
            IOrderService orderService,
            ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // GET: Order/MyOrders
        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _orderService.GetOrdersByCustomerAsync(userId);
            return View(orders.OrderByDescending(o => o.CreatedAt));
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("MyOrders");
            }

            // Verify user owns this order
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (order.CustomerId != userId)
            {
                return Forbid();
            }

            return View(order);
        }

        // POST: Order/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction("MyOrders");
                }

                // Verify user owns this order
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (order.CustomerId != userId)
                {
                    return Forbid();
                }

                var result = await _orderService.CancelOrderAsync(id);
                if (result)
                {
                    TempData["Success"] = "Order cancelled successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to cancel order.";
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId}", id);
                TempData["Error"] = "An error occurred while cancelling the order.";
            }

            return RedirectToAction("Details", new { id });
        }
    }
}
