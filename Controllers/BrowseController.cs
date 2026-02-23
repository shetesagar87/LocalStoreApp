using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CleanMvcApp.Services;

namespace CleanMvcApp.Controllers
{
    [Authorize]
    public class BrowseController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IStoreService _storeService;
        private readonly ILogger<BrowseController> _logger;

        public BrowseController(
            IProductService productService,
            ICategoryService categoryService,
            IStoreService storeService,
            ILogger<BrowseController> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _storeService = storeService;
            _logger = logger;
        }

        // Browse products by store
        public async Task<IActionResult> Products(int storeId, string? searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var store = await _storeService.GetStoreByIdAsync(storeId);
            if (store == null)
            {
                return NotFound();
            }

            ViewBag.Store = store;
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CategoryId = categoryId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            var products = await _productService.SearchProductsAsync(
                searchTerm: searchTerm,
                categoryId: categoryId,
                minPrice: minPrice,
                maxPrice: maxPrice,
                storeId: storeId,
                minRating: null
            );

            return View(products);
        }

        // Search all products across stores
        public async Task<IActionResult> Search(string? searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice, decimal? minRating)
        {
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CategoryId = categoryId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.MinRating = minRating;

            var products = await _productService.SearchProductsAsync(
                searchTerm: searchTerm,
                categoryId: categoryId,
                minPrice: minPrice,
                maxPrice: maxPrice,
                storeId: null,
                minRating: minRating
            );

            return View(products);
        }

        // Product details
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
