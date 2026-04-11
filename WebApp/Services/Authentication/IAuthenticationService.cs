using FakeStore.ViewModel;

namespace WebApp.Service;

public interface IAuthenticationService
{
    Task<AuthenticationResponse> login(LoginRequest loginRequest);
    Task<AuthenticationResponse> register(RegisterRequest registerRequest);
}
