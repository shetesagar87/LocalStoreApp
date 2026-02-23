using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CleanMvcApp.Models;
using CleanMvcApp.Services;
using Microsoft.AspNetCore.Authorization;

namespace CleanMvcApp.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IStoreService _storeService;

    public HomeController(ILogger<HomeController> logger, IStoreService storeService)
    {
        _logger = logger;
        _storeService = storeService;
    }

    public async Task<IActionResult> Index()
    {
        // For now, show all approved and active stores
        // In production, this would use user's location
        var stores = await _storeService.GetAllStoresAsync();
        var activeStores = stores.Where(s => s.Status == Models.Enums.StoreStatus.Approved && s.IsActive).ToList();
        
        return View(activeStores);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
