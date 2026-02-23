using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CleanMvcApp.Services;
using CleanMvcApp.Models.Enums;
using System.Security.Claims;

namespace CleanMvcApp.Controllers
{
    [Authorize(Roles = "StoreOwner")]
    public class StoreOrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IStoreService _storeService;
        private readonly ILogger<StoreOrderController> _logger;

        public StoreOrderController(
            IOrderService orderService,
            IStoreService storeService,
            ILogger<StoreOrderController> logger)
        {
            _orderService = orderService;
            _storeService = storeService;
            _logger = logger;
        }

        // GET: StoreOrder/Index
        public async Task<IActionResult> Index(string? status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Get store owned by this user
            var stores = await _storeService.GetStoresByOwnerAsync(userId);
            var store = stores.FirstOrDefault();

            if (store == null)
            {
                TempData["Error"] = "You don't have a store yet.";
                return RedirectToAction("Index", "Store");
            }

            // Get orders for this store
            var orders = await _orderService.GetOrdersByStoreAsync(store.StoreId);

            // Filter by status if provided
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var orderStatus))
            {
                orders = orders.Where(o => o.Status == orderStatus);
            }

            ViewBag.Store = store;
            ViewBag.SelectedStatus = status;
            return View(orders.OrderByDescending(o => o.CreatedAt));
        }

        // GET: StoreOrder/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Index");
            }

            // Verify user owns the store for this order
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var stores = await _storeService.GetStoresByOwnerAsync(userId);
            var store = stores.FirstOrDefault(s => s.StoreId == order.StoreId);

            if (store == null)
            {
                return Forbid();
            }

            return View(order);
        }

        // POST: StoreOrder/Accept/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction("Index");
                }

                // Verify user owns the store for this order
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var stores = await _storeService.GetStoresByOwnerAsync(userId);
                var store = stores.FirstOrDefault(s => s.StoreId == order.StoreId);

                if (store == null)
                {
                    return Forbid();
                }

                var result = await _orderService.AcceptOrderAsync(id);
                if (result)
                {
                    TempData["Success"] = "Order accepted successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to accept order.";
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting order {OrderId}", id);
                TempData["Error"] = "An error occurred while accepting the order.";
            }

            return RedirectToAction("Details", new { id });
        }

        // POST: StoreOrder/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction("Index");
                }

                // Verify user owns the store for this order
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var stores = await _storeService.GetStoresByOwnerAsync(userId);
                var store = stores.FirstOrDefault(s => s.StoreId == order.StoreId);

                if (store == null)
                {
                    return Forbid();
                }

                if (string.IsNullOrWhiteSpace(reason))
                {
                    TempData["Error"] = "Please provide a reason for rejection.";
                    return RedirectToAction("Details", new { id });
                }

                var result = await _orderService.RejectOrderAsync(id, reason);
                if (result)
                {
                    TempData["Success"] = "Order rejected successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to reject order.";
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting order {OrderId}", id);
                TempData["Error"] = "An error occurred while rejecting the order.";
            }

            return RedirectToAction("Details", new { id });
        }

        // POST: StoreOrder/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction("Index");
                }

                // Verify user owns the store for this order
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var stores = await _storeService.GetStoresByOwnerAsync(userId);
                var store = stores.FirstOrDefault(s => s.StoreId == order.StoreId);

                if (store == null)
                {
                    return Forbid();
                }

                if (!Enum.TryParse<OrderStatus>(status, out var orderStatus))
                {
                    TempData["Error"] = "Invalid order status.";
                    return RedirectToAction("Details", new { id });
                }

                var result = await _orderService.UpdateOrderStatusAsync(id, orderStatus);
                if (result)
                {
                    TempData["Success"] = $"Order status updated to {orderStatus}.";
                }
                else
                {
                    TempData["Error"] = "Failed to update order status.";
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId} status", id);
                TempData["Error"] = "An error occurred while updating the order status.";
            }

            return RedirectToAction("Details", new { id });
        }
    }
}
