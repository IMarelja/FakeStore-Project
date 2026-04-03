using FakeStore.ViewModel;

namespace MyRestApi.Services;

public interface IAuthenticationService
{
    Task<AuthenticationResponse> LoginAsync(LoginRequest req);
    Task<AuthenticationResponse> RegisterAsync(RegisterRequest req);
}