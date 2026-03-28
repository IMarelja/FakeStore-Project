using Microsoft.AspNetCore.Mvc;
using FakeStore.ViewModel;
using MyRestApi.Repositories;

namespace MyRestApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationRepo _repo;

    public AuthenticationController(IAuthenticationRepo repo)
    {
        _repo = repo;
    }

    // POST /api/authentication/login
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req)
    {
        var result = await _repo.LoginAsync(req);
        return result is null ? Unauthorized() : Ok(result);
    }

    // POST /api/authentication/register
    [HttpPost("register")]
    public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest req)
    {
        var result = await _repo.RegisterAsync(req);
        return CreatedAtAction(nameof(Login), result);
    }
}
