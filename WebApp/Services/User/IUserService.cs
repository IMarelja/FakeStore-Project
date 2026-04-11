using FakeStore.ViewModel;

namespace FakeStore.WebApp.Service;

public interface IUserService
{
    Task<IEnumerable<UserRead>> GetAll();
    Task<UserRead?> GetById(int id);
    Task<UserRead?> GetMe();
    Task<UserRead?> EditMe(UserUpdate user);
    Task<bool> DeleteMe();
    Task<UserRead?> EditUser(int id, UserUpdate user);
    Task<bool> DeleteUser(int id);
}
