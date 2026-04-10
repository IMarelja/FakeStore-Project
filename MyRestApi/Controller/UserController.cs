using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyRestApi.DTO.User;
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
    [Authorize(Roles = "read-only,full access")]
    public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    // GET /api/user/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = "read-only,full access")]
    public async Task<ActionResult<UserReadDto>> GetById(int id)
    {
        var user = await _service.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    // GET /api/user/me
    [HttpGet("me/")]
    [Authorize(Roles = "read-only,full access")]
    public async Task<ActionResult<UserReadDto>> GetMe()
    {
        var user = await _service.GetByIdAsync(_claims.GetUserId());
        return user is null ? NotFound() : Ok(user);
    }

    // PUT /api/user/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "full access")]
    public async Task<ActionResult<UserReadDto>> EditUser(int id, [FromBody] UserUpdateDto req)
    {
        var updated = await _service.UpdateAsync(id, req);
        return updated is null ? NotFound() : Ok(updated);
    }

    // PUT /api/user/me
    [HttpPut("me/")]
    [Authorize(Roles = "full access")]
    public async Task<ActionResult<UserReadDto>> EditUserMe([FromBody] UserUpdateDto req)
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
