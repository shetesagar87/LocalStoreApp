using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CleanMvcApp.Services;

namespace CleanMvcApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly IAdminDashboardService _dashboardService;
        private readonly IStoreService _storeService;
        private readonly ILogger<AdminDashboardController> _logger;

        public AdminDashboardController(
            IAdminDashboardService dashboardService,
            IStoreService storeService,
            ILogger<AdminDashboardController> logger)
        {
            _dashboardService = dashboardService;
            _storeService = storeService;
            _logger = logger;
        }

        // GET: AdminDashboard
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.TotalSales = await _dashboardService.GetTotalSalesAsync();
                ViewBag.ActiveStores = await _dashboardService.GetActiveStoresCountAsync();
                ViewBag.TotalOrders = await _dashboardService.GetTotalOrdersCountAsync();
                ViewBag.TopProducts = await _dashboardService.GetTopSellingProductsAsync(10);
                ViewBag.MonthlyRevenue = await _dashboardService.GetMonthlyRevenueTrendsAsync(12);
                
                // Get pending stores
                var pendingStores = await _storeService.GetPendingStoresAsync();
                ViewBag.PendingStoresCount = pendingStores.Count();

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard");
                TempData["Error"] = "An error occurred while loading the dashboard.";
                return View();
            }
        }
    }
}
