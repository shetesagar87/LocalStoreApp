using CleanMvcApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace CleanMvcApp.Services
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterAsync(RegisterViewModel model, string role);
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
        Task<bool> SendEmailVerificationAsync(string userId);
        Task<IdentityResult> VerifyEmailAsync(string userId, string token);
    }
}
