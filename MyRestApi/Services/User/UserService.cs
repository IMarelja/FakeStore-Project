using FakeStore.Models;
using MyRestApi.DTO.User;
using MyRestApi.Repositories;

namespace MyRestApi.Services;

public class UserService : IUserService
{
    private readonly IUserRepo _repo;

    public UserService(IUserRepo repo)
    {
        _repo = repo;
    }

    public async Task<List<UserReadDto>> GetAllAsync()
    {
        var users = await _repo.GetAllAsync();
        return users.Select(ToDto).ToList();
    }

    public async Task<UserReadDto?> GetByIdAsync(int userId)
    {
        var user = await _repo.GetByIdAsync(userId);
        return user is null ? null : ToDto(user);
    }

    public async Task<UserReadDto?> UpdateAsync(int userId, UserUpdateDto req)
    {
        var user = await _repo.UpdateAsync(userId, req);
        return user is null ? null : ToDto(user);
    }

    public Task<bool> DeleteAsync(int userId) =>
        _repo.DeleteAsync(userId);

    private static UserReadDto ToDto(User u) => new()
    {
        user_id  = u.UserId,
        username = u.Username,
        email    = u.Email,
        password = u.Password
    };
}
