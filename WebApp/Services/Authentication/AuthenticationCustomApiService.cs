using FakeStore.ViewModel;

namespace FakeStore.WebApp.Service;

public class AuthenticationCustomApiService : IAuthenticationService
{
    public Task<AuthenticationResponse> login(LoginRequest loginRequest)
    {
        throw new NotImplementedException();
    }

    public Task<AuthenticationResponse> register(RegisterRequest registerRequest)
    {
        throw new NotImplementedException();
    }
}