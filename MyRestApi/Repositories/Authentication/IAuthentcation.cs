using FakeStore.Models;
using FakeStore.ViewModel;

namespace MyRestApi.Repositories;

public interface IAuthenticationRepo
{
    Task<User?> UserByEmailAsync(string email);
    Task<User?> UserByUsernameAsync(string username);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);
    Task<User> CreateUserAsync(RegisterRequest req);
}
