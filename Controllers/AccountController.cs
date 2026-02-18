using CleanMvcApp.Models.ViewModels;
using CleanMvcApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanMvcApp.Controllers;

public class AccountController : Controller
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAuthenticationService authenticationService, ILogger<AccountController> logger)
    {
        _authenticationService = authenticationService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var role = model.IsStoreOwner ? "StoreOwner" : "Customer";
        var result = await _authenticationService.RegisterAsync(model, role);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} registered successfully as {Role}", model.Email, role);
            TempData["SuccessMessage"] = "Registration successful! Please check your email to verify your account.";
            return RedirectToAction(nameof(Login));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authenticationService.LoginAsync(model);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} logged in successfully", model.Email);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning("User {Email} account locked out", model.Email);
            ModelState.AddModelError(string.Empty, "Account locked due to multiple failed login attempts. Please try again later.");
            return View(model);
        }

        if (result.IsNotAllowed)
        {
            _logger.LogWarning("User {Email} login not allowed - email not verified", model.Email);
            ModelState.AddModelError(string.Empty, "Please verify your email before logging in.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "Invalid email or password");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _authenticationService.LogoutAsync();
        _logger.LogInformation("User logged out");
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> VerifyEmail(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            return BadRequest("Invalid email verification link");
        }

        var result = await _authenticationService.VerifyEmailAsync(userId, token);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {UserId} email verified successfully", userId);
            TempData["SuccessMessage"] = "Email verified successfully! You can now log in.";
            return RedirectToAction(nameof(Login));
        }

        _logger.LogWarning("Email verification failed for user {UserId}", userId);
        TempData["ErrorMessage"] = "Email verification failed. The link may be invalid or expired.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
