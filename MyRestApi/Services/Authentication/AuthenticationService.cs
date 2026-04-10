using MyRestApi.DTO.Auth;
using MyRestApi.Middleware;
using MyRestApi.Repositories;

namespace MyRestApi.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationRepo _repo;
    private readonly IJwtService _jwtService;

    public AuthenticationService(IAuthenticationRepo repo, IJwtService jwtService)
    {
        _repo = repo;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest)
    {
        var user = await _repo.UserByUsernameAsync(loginRequest.username)
            ?? throw new UnauthorizedException("Invalid username or password.");

        if (user.Password != loginRequest.password)
            throw new UnauthorizedException("Invalid username or password.");

        return _jwtService.GenerateToken(user, loginRequest.remember_me);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
    {
        if (await _repo.EmailExistsAsync(registerRequest.email))
            throw new ConflictException($"'{registerRequest.email}' is already taken.");

        if (await _repo.UsernameExistsAsync(registerRequest.username))
            throw new ConflictException($"'{registerRequest.username}' is already taken.");

        var user = await _repo.CreateUserAsync(registerRequest);
        return _jwtService.GenerateToken(user, registerRequest.remember_me);
    }
}
