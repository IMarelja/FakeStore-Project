
using FakeStore.View;
using FakeStore.ViewModel;

namespace MyRestApi.Repositories;

public class ProductPublicApiRepo : IAuthenticationRepo
{
    public ProductPublicApiRepo()
    {
        
    }

    public Task<User> CreateUserAsync(RegisterRequest req)
    {
        throw new NotImplementedException();
    }

    public Task<bool> EmailExistsAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<User?> UserByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<User?> UserByUsernameAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UsernameExistsAsync(string username)
    {
        throw new NotImplementedException();
    }
}