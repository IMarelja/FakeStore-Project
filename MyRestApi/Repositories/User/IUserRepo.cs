using FakeStore.Models;
using FakeStore.ViewModel;

namespace MyRestApi.Repositories;

public interface IUserRepo
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int userId);
    Task<User?> UpdateAsync(int userId, UserUpdate req);
    Task<bool> DeleteAsync(int userId);
}
