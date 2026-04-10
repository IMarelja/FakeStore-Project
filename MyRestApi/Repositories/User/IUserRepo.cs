using FakeStore.Models;
using MyRestApi.DTO.User;

namespace MyRestApi.Repositories;

public interface IUserRepo
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int userId);
    Task<User?> UpdateAsync(int userId, UserUpdateDto req);
    Task<bool> DeleteAsync(int userId);
}
