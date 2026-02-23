using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CleanMvcApp.Models.Entities;

namespace CleanMvcApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminUserController> _logger;

        public AdminUserController(
            UserManager<ApplicationUser> userManager,
            ILogger<AdminUserController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        // GET: AdminUser/ListUsers
        public async Task<IActionResult> ListUsers(string? searchTerm, string? role)
        {
            var users = _userManager.Users.AsQueryable();

            // Filter by search term
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                users = users.Where(u => 
                    u.Email.Contains(searchTerm) || 
                    u.FullName.Contains(searchTerm));
            }

            var userList = users.OrderBy(u => u.Email).ToList();

            // Filter by role if specified
            if (!string.IsNullOrWhiteSpace(role))
            {
                var usersInRole = new List<ApplicationUser>();
                foreach (var user in userList)
                {
                    if (await _userManager.IsInRoleAsync(user, role))
                    {
                        usersInRole.Add(user);
                    }
                }
                userList = usersInRole;
            }

            // Get roles for each user
            var userViewModels = new List<(ApplicationUser User, IList<string> Roles)>();
            foreach (var user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add((user, roles));
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SelectedRole = role;
            return View(userViewModels);
        }

        // POST: AdminUser/DisableUser/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("ListUsers");
                }

                // Prevent disabling yourself
                if (user.Id == User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)
                {
                    TempData["Error"] = "You cannot disable your own account.";
                    return RedirectToAction("ListUsers");
                }

                // Set lockout end date to far future
                var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                if (result.Succeeded)
                {
                    TempData["Success"] = $"User {user.Email} has been disabled.";
                }
                else
                {
                    TempData["Error"] = "Failed to disable user.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling user {UserId}", id);
                TempData["Error"] = "An error occurred while disabling the user.";
            }

            return RedirectToAction("ListUsers");
        }

        // POST: AdminUser/EnableUser/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("ListUsers");
                }

                // Remove lockout
                var result = await _userManager.SetLockoutEndDateAsync(user, null);
                if (result.Succeeded)
                {
                    TempData["Success"] = $"User {user.Email} has been enabled.";
                }
                else
                {
                    TempData["Error"] = "Failed to enable user.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling user {UserId}", id);
                TempData["Error"] = "An error occurred while enabling the user.";
            }

            return RedirectToAction("ListUsers");
        }
    }
}
