using Microsoft.AspNetCore.Mvc;
using MyRestApi.DTO.Auth;
using MyRestApi.Services;

namespace MyRestApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _service;

    public AuthenticationController(IAuthenticationService service)
    {
        _service = service;
    }

    // POST /api/authentication/login
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto req)
    {
        return Ok(await _service.LoginAsync(req));
    }

    // POST /api/authentication/register
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto req)
    {
        var result = await _service.RegisterAsync(req);
        return CreatedAtAction(nameof(Login), result);
    }
}
