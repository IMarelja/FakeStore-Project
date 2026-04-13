using FakeStore.ViewModel;
using FakeStore.WebApp.Configuration;
using FakeStore.WebApp.Models;
using FakeStore.WebApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FakeStore.WebApp.Controllers;

public class UserController : Controller
{
    private const string UserCreateSuccessKey = "UserCreateSuccess";
    private const string UserEditSuccessKey = "UserEditSuccess";
    private const string UserDeleteSuccessKey = "UserDeleteSuccess";

    private readonly IUserService _userService;
    private readonly IAuthenticationService _authenticationService;
    private readonly ApiRuntimeMode _apiRuntimeMode;

    public UserController(
        IUserService userService,
        IAuthenticationService authenticationService,
        ApiRuntimeMode apiRuntimeMode)
    {
        _userService = userService;
        _authenticationService = authenticationService;
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

    [HttpGet]
    [Authorize]
    [Authorize(Policy = "CustomApiOnly")]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public IActionResult Create()
    {
        var vm = BuildCreatePageVm(
            successMessage: ReadTempDataMessage(UserCreateSuccessKey));
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [Authorize(Policy = "CustomApiOnly")]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public async Task<IActionResult> Create(UserFormInputModel form)
    {
        var vm = BuildCreatePageVm(form);
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        try
        {
            var auth = await _authenticationService.register(new RegisterRequest
            {
                username = form.Username.Trim(),
                email = form.Email.Trim(),
                password = form.Password,
                remember_me = false
            });

            if (string.IsNullOrWhiteSpace(auth.token))
            {
                vm.ErrorMessage = "User creation failed.";
                return View(vm);
            }

            TempData[UserCreateSuccessKey] = "User created successfully.";
            return RedirectToAction(nameof(Create));
        }
        catch (Exception ex)
        {
            vm.ErrorMessage = BuildErrorMessage("User creation failed.", ex);
            return View(vm);
        }
    }

    [HttpGet]
    [Authorize]
    [Authorize(Policy = "CustomApiOnly")]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public async Task<IActionResult> Edit(int id)
    {
        UserRead? user;
        try
        {
            user = await _userService.GetById(id);
        }
        catch (Exception ex)
        {
            return View(BuildEditPageVm(
                id,
                new UserFormInputModel(),
                errorMessage: BuildErrorMessage("Could not load user data.", ex)));
        }

        if (user is null)
        {
            return NotFound();
        }

        var vm = BuildEditPageVm(
            id,
            new UserFormInputModel
            {
                Username = user.username,
                Email = user.email,
                Password = user.password
            },
            successMessage: ReadTempDataMessage(UserEditSuccessKey));
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [Authorize(Policy = "CustomApiOnly")]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public async Task<IActionResult> Edit(int id, UserFormInputModel form)
    {
        var vm = BuildEditPageVm(id, form);
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        try
        {
            var updated = await _userService.EditUser(id, new UserUpdate
            {
                Username = form.Username.Trim(),
                Email = form.Email.Trim(),
                Password = form.Password
            });

            if (updated is null)
            {
                vm.ErrorMessage = "User not found.";
                return View(vm);
            }

            TempData[UserEditSuccessKey] = "User updated successfully.";
            return RedirectToAction(nameof(Edit), new { id });
        }
        catch (Exception ex)
        {
            vm.ErrorMessage = BuildErrorMessage("User update failed.", ex);
            return View(vm);
        }
    }

    [HttpGet]
    [Authorize]
    [Authorize(Policy = "CustomApiOnly")]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public async Task<IActionResult> Delete(int id, bool deleted = false)
    {
        if (deleted)
        {
            return View(new UserDeletePageViewModel
            {
                Deleted = true,
                SuccessMessage = ReadTempDataMessage(UserDeleteSuccessKey) ?? "User deleted successfully."
            });
        }

        try
        {
            var user = await _userService.GetById(id);
            if (user is null)
            {
                return NotFound();
            }

            return View(new UserDeletePageViewModel
            {
                User = user
            });
        }
        catch (Exception ex)
        {
            return View(new UserDeletePageViewModel
            {
                ErrorMessage = BuildErrorMessage("Could not load user data.", ex)
            });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [Authorize(Policy = "CustomApiOnly")]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var deleted = await _userService.DeleteUser(id);
            if (!deleted)
            {
                return View("Delete", new UserDeletePageViewModel
                {
                    ErrorMessage = "User not found."
                });
            }

            TempData[UserDeleteSuccessKey] = "User deleted successfully.";
            return RedirectToAction(nameof(Delete), new { id, deleted = true });
        }
        catch (Exception ex)
        {
            return View("Delete", new UserDeletePageViewModel
            {
                ErrorMessage = BuildErrorMessage("User deletion failed.", ex)
            });
        }
    }

    private UserFormPageViewModel BuildCreatePageVm(
        UserFormInputModel? form = null,
        string? successMessage = null,
        string? errorMessage = null)
    {
        return new UserFormPageViewModel
        {
            Title = "Create User",
            SubmitLabel = "Create User",
            Form = form ?? new UserFormInputModel(),
            SuccessMessage = successMessage,
            ErrorMessage = errorMessage
        };
    }

    private UserFormPageViewModel BuildEditPageVm(
        int id,
        UserFormInputModel form,
        string? successMessage = null,
        string? errorMessage = null)
    {
        return new UserFormPageViewModel
        {
            UserId = id,
            Title = $"Edit User #{id}",
            SubmitLabel = "Save Changes",
            Form = form,
            SuccessMessage = successMessage,
            ErrorMessage = errorMessage
        };
    }

    private string? ReadTempDataMessage(string key)
    {
        return TempData[key]?.ToString();
    }

    private static string BuildErrorMessage(string prefix, Exception ex)
    {
        if (ex is HttpRequestException requestException && !string.IsNullOrWhiteSpace(requestException.Message))
        {
            return $"{prefix} {requestException.Message}";
        }

        return prefix;
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
