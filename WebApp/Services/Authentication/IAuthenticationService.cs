using FakeStore.ViewModel;

namespace FakeStore.WebApp.Service;

public interface IAuthenticationService
{
    Task<AuthenticationResponse> login(LoginRequest loginRequest);
    Task<AuthenticationResponse> register(RegisterRequest registerRequest);
}
