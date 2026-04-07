using FakeStore.ViewModel;
using Microsoft.AspNetCore.Mvc;
using MyRestApi.Services;

namespace MyRestApi.Controller;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
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

    // PUT /api/user/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<UserRead>> EditUser(int id, [FromBody] UserUpdate req)
    {
        var updated = await _service.UpdateAsync(id, req);
        return updated is null ? NotFound() : Ok(updated);
    }

    // DELETE /api/user/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        return await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
