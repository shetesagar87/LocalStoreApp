using CleanMvcApp.Models;
using System.Text.Json;

namespace CleanMvcApp.Services;

public class AuthService : IAuthService
{
    private readonly string _userFilePath;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IWebHostEnvironment env, ILogger<AuthService> logger)
    {
        _userFilePath = Path.Combine(env.ContentRootPath, "Data", "users.json");
        _logger = logger;
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        try
        {
            var users = await GetAllUsersAsync();
            var user = users.FirstOrDefault(u => 
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
                u.Password == password);

            if (user != null)
            {
                _logger.LogInformation("User {Username} authenticated successfully", username);
            }
            else
            {
                _logger.LogWarning("Failed authentication attempt for username: {Username}", username);
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during authentication");
            return null;
        }
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        try
        {
            if (!File.Exists(_userFilePath))
            {
                _logger.LogError("User data file not found at {Path}", _userFilePath);
                return new List<User>();
            }

            var json = await File.ReadAllTextAsync(_userFilePath);
            var userData = JsonSerializer.Deserialize<UserData>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return userData?.Users ?? new List<User>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading user data");
            return new List<User>();
        }
    }
}
