using FakeStore.Models;
using FakeStore.ViewModel;
using MyRestApi.Middleware;

namespace MyRestApi.Repositories;

public class UserGraphQLRepo : IUserRepo
{
    public Task<List<User>> GetAllAsync() =>
        throw new Exception("GetAll users is not yet implemented.");

    public Task<User?> GetByIdAsync(int userId) =>
        throw new Exception("GetById user is not yet implemented.");

    public Task<User?> UpdateAsync(int userId, UserUpdate req) =>
        throw new Exception("UpdateUser is not yet implemented.");

    public Task<bool> DeleteAsync(int userId) =>
        throw new Exception("DeleteUser is not yet implemented.");
}
