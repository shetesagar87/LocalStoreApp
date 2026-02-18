using CleanMvcApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanMvcApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminStoreController : Controller
    {
        private readonly IStoreService _storeService;
        private readonly ILogger<AdminStoreController> _logger;

        public AdminStoreController(IStoreService storeService, ILogger<AdminStoreController> logger)
        {
            _storeService = storeService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var stores = await _storeService.GetAllStoresAsync();
            return View(stores);
        }

        [HttpGet]
        public async Task<IActionResult> Pending()
        {
            var stores = await _storeService.GetPendingStoresAsync();
            return View(stores);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var result = await _storeService.ApproveStoreAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Store approved successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to approve store.";
                }

                return RedirectToAction(nameof(Pending));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving store {StoreId}", id);
                TempData["ErrorMessage"] = "An error occurred while approving the store.";
                return RedirectToAction(nameof(Pending));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            try
            {
                var result = await _storeService.RejectStoreAsync(id, reason);
                if (result)
                {
                    TempData["SuccessMessage"] = "Store rejected successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to reject store.";
                }

                return RedirectToAction(nameof(Pending));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting store {StoreId}", id);
                TempData["ErrorMessage"] = "An error occurred while rejecting the store.";
                return RedirectToAction(nameof(Pending));
            }
        }
    }
}
