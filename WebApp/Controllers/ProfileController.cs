using FakeStore.ViewModel;
using FakeStore.WebApp.Models;
using FakeStore.WebApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FakeStore.WebApp.Controllers;

[Authorize]
[Authorize(Policy = "CustomApiOnly")]
[Authorize(Policy = "ReadOnlyRole")]
public class ProfileController : Controller
{
    private readonly IUserService _userService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IJwtService _jwtService;

    public ProfileController(IUserService userService, IAuthorizationService authorizationService, IJwtService jwtService)
    {
        _userService = userService;
        _authorizationService = authorizationService;
        _jwtService = jwtService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await BuildPageViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public async Task<IActionResult> Update(string username, string email, string password)
    {
        var request = new UserUpdate
        {
            Username = username,
            Email = email,
            Password = password
        };

        if (string.IsNullOrWhiteSpace(request.Username)
            || string.IsNullOrWhiteSpace(request.Email)
            || string.IsNullOrWhiteSpace(request.Password))
        {
            var invalidVm = await BuildPageViewModel(errorMessage: "Username, email and password are required.");
            invalidVm.UpdateRequest = request;
            return View("Index", invalidVm);
        }

        try
        {
            var updated = await _userService.EditMe(request);
            if (updated is null)
            {
                var missingVm = await BuildPageViewModel(errorMessage: "Profile could not be updated.");
                missingVm.UpdateRequest = request;
                return View("Index", missingVm);
            }

            var vm = await BuildPageViewModel(successMessage: "Profile updated successfully.");
            vm.UpdateRequest = new UserUpdate
            {
                Username = updated.username,
                Email = updated.email,
                Password = updated.password
            };

            return View("Index", vm);
        }
        catch
        {
            var failedVm = await BuildPageViewModel(errorMessage: "Profile update failed.");
            failedVm.UpdateRequest = request;
            return View("Index", failedVm);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public async Task<IActionResult> DeleteMe()
    {
        try
        {
            var deleted = await _userService.DeleteMe();
            if (!deleted)
            {
                return View("Index", await BuildPageViewModel(errorMessage: "Profile could not be deleted."));
            }

            await _jwtService.DeleteTokenAsync();
            return RedirectToAction("Index", "Home");
        }
        catch
        {
            return View("Index", await BuildPageViewModel(errorMessage: "Profile deletion failed."));
        }
    }

    private async Task<ProfilePageViewModel> BuildPageViewModel(string? successMessage = null, string? errorMessage = null)
    {
        UserRead? me = null;

        try
        {
            me = await _userService.GetMe();
        }
        catch
        {
            errorMessage ??= "Could not load profile data.";
        }

        return new ProfilePageViewModel
        {
            CurrentUser = me,
            SuccessMessage = successMessage,
            ErrorMessage = errorMessage,
            UpdateRequest = new UserUpdate
            {
                Username = me?.username,
                Email = me?.email,
                Password = me?.password
            }
        };
    }
}
