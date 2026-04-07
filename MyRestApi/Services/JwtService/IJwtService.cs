using FakeStore.ViewModel;
using FakeStore.Models;

namespace MyRestApi.Services;

public interface IJwtService
{
    AuthenticationResponse GenerateToken(User user, bool rememberMe);
}
