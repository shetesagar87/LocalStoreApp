using CleanMvcApp.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanMvcApp.Controllers;

[Authorize]
public class AdminController : Controller
{
    [Permission("ManageUsers")]
    public IActionResult ManageUsers()
    {
        return View();
    }

    [Permission("ViewSettings")]
    public IActionResult Settings()
    {
        return View();
    }
}
