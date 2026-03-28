using FakeStore.ViewModel;

namespace MyRestApi.Repositories;

public interface IAuthenticationRepo
{
    Task<LoginResponse?> LoginAsync(LoginRequest req);
    Task<LoginResponse> RegisterAsync(RegisterRequest req);
}
