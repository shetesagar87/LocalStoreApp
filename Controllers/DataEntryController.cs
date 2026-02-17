using CleanMvcApp.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanMvcApp.Controllers;

[Authorize]
public class DataEntryController : Controller
{
    [Permission("EditData")]
    public IActionResult Index()
    {
        return View();
    }
}
