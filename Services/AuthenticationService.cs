using CleanMvcApp.Models.Entities;
using CleanMvcApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace CleanMvcApp.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AuthenticationService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model, string role)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return IdentityResult.Failed(new IdentityError 
                    { 
                        Code = "DuplicateEmail", 
                        Description = "Email is already registered" 
                    });
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Assign role
                    await _userManager.AddToRoleAsync(user, role);
                    
                    _logger.LogInformation("User {Email} registered successfully with role {Role}", 
                        model.Email, role);
                }
                else
                {
                    _logger.LogWarning("Failed to register user {Email}: {Errors}", 
                        model.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user {Email}", model.Email);
                return IdentityResult.Failed(new IdentityError 
                { 
                    Code = "RegistrationError", 
                    Description = "An error occurred during registration" 
                });
            }
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                
                if (user == null)
                {
                    _logger.LogWarning("Login attempt for non-existent user {Email}", model.Email);
                    return SignInResult.Failed;
                }

                // Check if email is confirmed
                if (!user.EmailConfirmed)
                {
                    _logger.LogWarning("Login attempt for unverified user {Email}", model.Email);
                    return SignInResult.NotAllowed;
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    _logger.LogWarning("Login attempt for inactive user {Email}", model.Email);
                    return SignInResult.NotAllowed;
                }

                var result = await _signInManager.PasswordSignInAsync(
                    user, 
                    model.Password, 
                    model.RememberMe, 
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User {Email} logged in successfully", model.Email);
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning("User {Email} account locked out", model.Email);
                }
                else
                {
                    _logger.LogWarning("Failed login attempt for user {Email}", model.Email);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while logging in user {Email}", model.Email);
                return SignInResult.Failed;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during logout");
                throw;
            }
        }

        public async Task<bool> SendEmailVerificationAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Attempted to send verification email for non-existent user {UserId}", userId);
                    return false;
                }

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                
                // TODO: Implement email sending logic here
                // For now, just log the token
                _logger.LogInformation("Email verification token generated for user {Email}: {Token}", 
                    user.Email, token);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending verification email for user {UserId}", userId);
                return false;
            }
        }

        public async Task<IdentityResult> VerifyEmailAsync(string userId, string token)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Attempted to verify email for non-existent user {UserId}", userId);
                    return IdentityResult.Failed(new IdentityError 
                    { 
                        Code = "UserNotFound", 
                        Description = "User not found" 
                    });
                }

                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Email verified successfully for user {Email}", user.Email);
                }
                else
                {
                    _logger.LogWarning("Failed to verify email for user {Email}: {Errors}", 
                        user.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while verifying email for user {UserId}", userId);
                return IdentityResult.Failed(new IdentityError 
                { 
                    Code = "VerificationError", 
                    Description = "An error occurred during email verification" 
                });
            }
        }
    }
}
