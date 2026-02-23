using CleanMvcApp.Models.Entities;
using CleanMvcApp.Models.ViewModels;
using CleanMvcApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleanMvcApp.Controllers
{
    [Authorize(Roles = "StoreOwner")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IStoreService _storeService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProductController> _logger;
        private readonly IWebHostEnvironment _environment;

        public ProductController(
            IProductService productService,
            IStoreService storeService,
            ICategoryService categoryService,
            UserManager<ApplicationUser> userManager,
            ILogger<ProductController> logger,
            IWebHostEnvironment environment)
        {
            _productService = productService;
            _storeService = storeService;
            _categoryService = categoryService;
            _userManager = userManager;
            _logger = logger;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int storeId)
        {
            var userId = _userManager.GetUserId(User);
            var store = await _storeService.GetStoreByIdAsync(storeId);
            
            if (store == null || store.OwnerId != userId)
            {
                return Forbid();
            }

            var products = await _productService.GetProductsByStoreAsync(storeId);
            ViewBag.Store = store;
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int storeId)
        {
            var userId = _userManager.GetUserId(User);
            var store = await _storeService.GetStoreByIdAsync(storeId);
            
            if (store == null || store.OwnerId != userId)
            {
                return Forbid();
            }

            await PopulateCategoriesDropdown();
            ViewBag.StoreId = storeId;
            ViewBag.StoreName = store.StoreName;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int storeId, CreateProductViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            var store = await _storeService.GetStoreByIdAsync(storeId);
            
            if (store == null || store.OwnerId != userId)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                await PopulateCategoriesDropdown();
                ViewBag.StoreId = storeId;
                ViewBag.StoreName = store.StoreName;
                return View(model);
            }

            try
            {
                string? imageUrl = null;
                if (model.ProductImage != null)
                {
                    imageUrl = await SaveProductImageAsync(model.ProductImage);
                }

                var product = new Product
                {
                    ProductName = model.ProductName,
                    Description = model.Description,
                    SKU = model.SKU,
                    Price = model.Price,
                    DiscountPercentage = model.DiscountPercentage,
                    StockQuantity = model.StockQuantity,
                    CategoryId = model.CategoryId,
                    StoreId = storeId,
                    ProductImageUrl = imageUrl
                };

                await _productService.CreateProductAsync(product);

                TempData["SuccessMessage"] = "Product created successfully!";
                return RedirectToAction(nameof(Index), new { storeId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateCategoriesDropdown();
                ViewBag.StoreId = storeId;
                ViewBag.StoreName = store.StoreName;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the product.");
                await PopulateCategoriesDropdown();
                ViewBag.StoreId = storeId;
                ViewBag.StoreName = store.StoreName;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var store = await _storeService.GetStoreByIdAsync(product.StoreId);
            
            if (store == null || store.OwnerId != userId)
            {
                return Forbid();
            }

            var model = new CreateProductViewModel
            {
                ProductName = product.ProductName,
                Description = product.Description,
                SKU = product.SKU,
                Price = product.Price,
                DiscountPercentage = product.DiscountPercentage,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId
            };

            await PopulateCategoriesDropdown();
            ViewBag.ProductId = id;
            ViewBag.StoreId = product.StoreId;
            ViewBag.CurrentImageUrl = product.ProductImageUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateProductViewModel model)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var store = await _storeService.GetStoreByIdAsync(product.StoreId);
            
            if (store == null || store.OwnerId != userId)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                await PopulateCategoriesDropdown();
                ViewBag.ProductId = id;
                ViewBag.StoreId = product.StoreId;
                return View(model);
            }

            try
            {
                product.ProductName = model.ProductName;
                product.Description = model.Description;
                product.SKU = model.SKU;
                product.Price = model.Price;
                product.DiscountPercentage = model.DiscountPercentage;
                product.StockQuantity = model.StockQuantity;
                product.CategoryId = model.CategoryId;

                if (model.ProductImage != null)
                {
                    product.ProductImageUrl = await SaveProductImageAsync(model.ProductImage);
                }

                await _productService.UpdateProductAsync(product);

                TempData["SuccessMessage"] = "Product updated successfully!";
                return RedirectToAction(nameof(Index), new { storeId = product.StoreId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", id);
                ModelState.AddModelError(string.Empty, "An error occurred while updating the product.");
                await PopulateCategoriesDropdown();
                ViewBag.ProductId = id;
                ViewBag.StoreId = product.StoreId;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction(nameof(Index));
                }

                var userId = _userManager.GetUserId(User);
                var store = await _storeService.GetStoreByIdAsync(product.StoreId);
                
                if (store == null || store.OwnerId != userId)
                {
                    return Forbid();
                }

                var result = await _productService.DeleteProductAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Product deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete product.";
                }

                return RedirectToAction(nameof(Index), new { storeId = product.StoreId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the product.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var store = await _storeService.GetStoreByIdAsync(product.StoreId);
            
            if (store == null || store.OwnerId != userId)
            {
                return Forbid();
            }

            return View(product);
        }

        private async Task PopulateCategoriesDropdown()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
        }

        private async Task<string> SaveProductImageAsync(IFormFile image)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "products");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return $"/images/products/{uniqueFileName}";
        }
    }
}
