using FakeStore.WebApp.Configuration;
using FakeStore.WebApp.Models;
using FakeStore.WebApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace FakeStore.WebApp.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly ApiRuntimeMode _apiRuntimeMode;

    public UserController(IUserService userService, ApiRuntimeMode apiRuntimeMode)
    {
        _userService = userService;
        _apiRuntimeMode = apiRuntimeMode;
    }

    [HttpGet]
    public async Task<IActionResult> TabData()
    {
        if (!CanAccessTabScreen())
        {
            return StatusCode(StatusCodes.Status403Forbidden, "You need to sign-up to view this page");
        }

        try
        {
            var users = await _userService.GetAll();
            var vm = new UserTabCardsViewModel
            {
                Users = users,
                HasFullAccessRole = HasFullAccessRole()
            };

            return PartialView("_UserCards", vm);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Could not load users.");
        }
    }

    private bool CanAccessTabScreen()
    {
        var isSignedIn = User?.Identity?.IsAuthenticated ?? false;

        return _apiRuntimeMode.IsPublicMode || (_apiRuntimeMode.IsCustomMode && isSignedIn);
    }

    private bool HasFullAccessRole()
    {
        return User?.Claims.Any(c =>
            c.Type == System.Security.Claims.ClaimTypes.Role
            && c.Value.Equals("full access", StringComparison.OrdinalIgnoreCase)) ?? false;
    }
}
