using FakeStore.Models;
using MyRestApi.DTO.Auth;

namespace MyRestApi.Services;

public interface IJwtService
{
    AuthResponseDto GenerateToken(User user, bool rememberMe);
}
