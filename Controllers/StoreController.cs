using CleanMvcApp.Models.Entities;
using CleanMvcApp.Models.ViewModels;
using CleanMvcApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleanMvcApp.Controllers
{
    [Authorize(Roles = "StoreOwner")]
    public class StoreController : Controller
    {
        private readonly IStoreService _storeService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<StoreController> _logger;
        private readonly IWebHostEnvironment _environment;

        public StoreController(
            IStoreService storeService,
            UserManager<ApplicationUser> userManager,
            ILogger<StoreController> logger,
            IWebHostEnvironment environment)
        {
            _storeService = storeService;
            _userManager = userManager;
            _logger = logger;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var stores = await _storeService.GetStoresByOwnerAsync(userId);
            return View(stores);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStoreViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                string? imageUrl = null;
                if (model.StoreImage != null)
                {
                    imageUrl = await SaveStoreImageAsync(model.StoreImage);
                }

                var store = new Store
                {
                    StoreName = model.StoreName,
                    Address = model.Address,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    LicenseNumber = model.LicenseNumber,
                    DeliveryRadius = model.DeliveryRadius,
                    DeliveryCharge = model.DeliveryCharge,
                    StoreImageUrl = imageUrl,
                    OwnerId = userId
                };

                await _storeService.CreateStoreAsync(store);

                TempData["SuccessMessage"] = "Store registered successfully! Waiting for admin approval.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating store");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the store.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (store.OwnerId != userId)
            {
                return Forbid();
            }

            return View(store);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (store.OwnerId != userId)
            {
                return Forbid();
            }

            var model = new CreateStoreViewModel
            {
                StoreName = store.StoreName,
                Address = store.Address,
                Latitude = store.Latitude ?? 0,
                Longitude = store.Longitude ?? 0,
                LicenseNumber = store.LicenseNumber,
                DeliveryRadius = store.DeliveryRadius,
                DeliveryCharge = store.DeliveryCharge
            };

            ViewBag.StoreId = id;
            ViewBag.CurrentImageUrl = store.StoreImageUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateStoreViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StoreId = id;
                return View(model);
            }

            try
            {
                var store = await _storeService.GetStoreByIdAsync(id);
                if (store == null)
                {
                    return NotFound();
                }

                var userId = _userManager.GetUserId(User);
                if (store.OwnerId != userId)
                {
                    return Forbid();
                }

                store.StoreName = model.StoreName;
                store.Address = model.Address;
                store.Latitude = model.Latitude;
                store.Longitude = model.Longitude;
                store.LicenseNumber = model.LicenseNumber;
                store.DeliveryRadius = model.DeliveryRadius;
                store.DeliveryCharge = model.DeliveryCharge;

                if (model.StoreImage != null)
                {
                    store.StoreImageUrl = await SaveStoreImageAsync(model.StoreImage);
                }

                await _storeService.UpdateStoreAsync(store);

                TempData["SuccessMessage"] = "Store updated successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating store {StoreId}", id);
                ModelState.AddModelError(string.Empty, "An error occurred while updating the store.");
                ViewBag.StoreId = id;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var store = await _storeService.GetStoreByIdAsync(id);
                if (store == null)
                {
                    return NotFound();
                }

                var userId = _userManager.GetUserId(User);
                if (store.OwnerId != userId)
                {
                    return Forbid();
                }

                var result = await _storeService.ToggleStoreStatusAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Store status updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update store status.";
                }

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling store {StoreId} status", id);
                TempData["ErrorMessage"] = "An error occurred while updating store status.";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<string> SaveStoreImageAsync(IFormFile image)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "stores");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return $"/images/stores/{uniqueFileName}";
        }
    }
}
