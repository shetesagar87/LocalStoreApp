using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CleanMvcApp.Services;
using System.Security.Claims;

namespace CleanMvcApp.Controllers
{
    [Authorize(Roles = "StoreOwner")]
    public class StoreDashboardController : Controller
    {
        private readonly IStoreDashboardService _dashboardService;
        private readonly IStoreService _storeService;
        private readonly ILogger<StoreDashboardController> _logger;

        public StoreDashboardController(
            IStoreDashboardService dashboardService,
            IStoreService storeService,
            ILogger<StoreDashboardController> logger)
        {
            _dashboardService = dashboardService;
            _storeService = storeService;
            _logger = logger;
        }

        // GET: StoreDashboard
        public async Task<IActionResult> Index()
        {
            try
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
                    return RedirectToAction("Create", "Store");
                }

                ViewBag.Store = store;
                ViewBag.DailySales = await _dashboardService.GetDailySalesAsync(store.StoreId);
                ViewBag.PendingOrders = await _dashboardService.GetPendingOrdersCountAsync(store.StoreId);
                ViewBag.LowStockProducts = await _dashboardService.GetLowStockProductsAsync(store.StoreId);
                
                var monthlyStats = await _dashboardService.GetMonthlyStatsAsync(store.StoreId);
                ViewBag.MonthlyRevenue = monthlyStats.TotalRevenue;
                ViewBag.MonthlyOrders = monthlyStats.TotalOrders;
                ViewBag.AverageOrderValue = monthlyStats.AverageOrderValue;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading store dashboard");
                TempData["Error"] = "An error occurred while loading the dashboard.";
                return View();
            }
        }
    }
}
