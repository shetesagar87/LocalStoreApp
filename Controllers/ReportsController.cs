using CleanMvcApp.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanMvcApp.Controllers;

[Authorize]
public class ReportsController : Controller
{
    [Permission("ViewReports")]
    public IActionResult Index()
    {
        return View();
    }
}
