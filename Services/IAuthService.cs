using CleanMvcApp.Models;

namespace CleanMvcApp.Services;

public interface IAuthService
{
    Task<User?> AuthenticateAsync(string username, string password);
    Task<List<User>> GetAllUsersAsync();
}
