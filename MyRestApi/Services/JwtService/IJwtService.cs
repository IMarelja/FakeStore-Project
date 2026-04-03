using FakeStore.ViewModel;
using FakeStore.View;

namespace MyRestApi.Services;

public interface IJwtService
{
    AuthenticationResponse GenerateToken(User user, bool rememberMe);
}
