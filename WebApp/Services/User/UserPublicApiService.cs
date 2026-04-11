using FakeStore.ViewModel;

namespace WebApp.Service;

public class UserPublicApiService : IUserService
{
    public Task<bool> DeleteMe()
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUser(int id)
    {
        throw new NotImplementedException();
    }

    public Task<UserRead?> EditMe(UserUpdate user)
    {
        throw new NotImplementedException();
    }

    public Task<UserRead?> EditUser(int id, UserUpdate user)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserRead>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<UserRead?> GetById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<UserRead?> GetMe()
    {
        throw new NotImplementedException();
    }
}