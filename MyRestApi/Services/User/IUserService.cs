using FakeStore.ViewModel;

namespace MyRestApi.Services;

public interface IUserService
{
    Task<List<UserRead>> GetAllAsync();
    Task<UserRead?> GetByIdAsync(int userId);
    Task<UserRead?> UpdateAsync(int userId, UserUpdate req);
    Task<bool> DeleteAsync(int userId);
}
