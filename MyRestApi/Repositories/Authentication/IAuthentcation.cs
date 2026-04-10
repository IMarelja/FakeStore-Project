using FakeStore.Models;
using MyRestApi.DTO.Auth;

namespace MyRestApi.Repositories;

public interface IAuthenticationRepo
{
    Task<User?> UserByEmailAsync(string email);
    Task<User?> UserByUsernameAsync(string username);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);
    Task<User> CreateUserAsync(RegisterRequestDto req);
}
