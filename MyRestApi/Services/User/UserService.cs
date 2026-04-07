using FakeStore.Models;
using FakeStore.ViewModel;
using MyRestApi.Repositories;

namespace MyRestApi.Services;

public class UserService : IUserService
{
    private readonly IUserRepo _repo;

    public UserService(IUserRepo repo)
    {
        _repo = repo;
    }

    public async Task<List<UserRead>> GetAllAsync()
    {
        var users = await _repo.GetAllAsync();
        return users.Select(ToViewModel).ToList();
    }

    public async Task<UserRead?> GetByIdAsync(int userId)
    {
        var user = await _repo.GetByIdAsync(userId);
        return user is null ? null : ToViewModel(user);
    }

    public async Task<UserRead?> UpdateAsync(int userId, UserUpdate req)
    {
        var user = await _repo.UpdateAsync(userId, req);
        return user is null ? null : ToViewModel(user);
    }

    public Task<bool> DeleteAsync(int userId) =>
        _repo.DeleteAsync(userId);

    private static UserRead ToViewModel(User u) => new()
    {
        user_id  = u.UserId,
        username = u.Username,
        email    = u.Email,
        password = u.Password
    };
}
