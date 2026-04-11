using FakeStore.ViewModel;

namespace FakeStore.WebApp.Service;

public class UserCustomApiService : IUserService
{


    

    public async Task<bool> DeleteMe()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteUser(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<UserRead?> EditMe(UserUpdate user)
    {
        throw new NotImplementedException();
    }

    public async Task<UserRead?> EditUser(int id, UserUpdate user)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<UserRead>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<UserRead?> GetById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<UserRead?> GetMe()
    {
        throw new NotImplementedException();
    }
}