using MyRestApi.DTO.Auth;

namespace MyRestApi.Services;

public interface IAuthenticationService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto req);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto req);
}
