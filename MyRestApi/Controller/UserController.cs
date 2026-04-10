using FakeStore.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyRestApi.Services;

namespace MyRestApi.Controller;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _service;
    private readonly IClaimsService _claims;

    public UserController(IUserService service, IClaimsService claims)
    {
        _service = service;
        _claims = claims;
    }

    // GET /api/user
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserRead>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    // GET /api/user/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<UserRead>> GetById(int id)
    {
        var user = await _service.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    // GET /api/user/me
    [HttpGet("me/")]
    public async Task<ActionResult<UserRead>> GetMe()
    {
        var user = await _service.GetByIdAsync(_claims.GetUserId());
        return user is null ? NotFound() : Ok(user);
    }

    // PUT /api/user/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "full access")]
    public async Task<ActionResult<UserRead>> EditUser(int id, [FromBody] UserUpdate req)
    {
        var updated = await _service.UpdateAsync(id, req);
        return updated is null ? NotFound() : Ok(updated);
    }

    // PUT /api/user/me
    [HttpPut("me/")]
    [Authorize(Roles = "full access")]
    public async Task<ActionResult<UserRead>> EditUserMe([FromBody] UserUpdate req)
    {
        var updated = await _service.UpdateAsync(_claims.GetUserId(), req);
        return updated is null ? NotFound() : Ok(updated);
    }

    // DELETE /api/user/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "full access")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        return await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }

    // DELETE /api/user/me
    [HttpDelete("me/")]
    [Authorize(Roles = "full access")]
    public async Task<IActionResult> DeleteUserMe()
    {
        return await _service.DeleteAsync(_claims.GetUserId()) ? NoContent() : NotFound();
    }
}
