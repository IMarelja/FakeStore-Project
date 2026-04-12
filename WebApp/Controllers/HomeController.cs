using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FakeStore.WebApp.Models;
using FakeStore.WebApp.Configuration;
using System.Security.Claims;

namespace FakeStore.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ApiRuntimeMode _apiRuntimeMode;

    public HomeController(
        ApiRuntimeMode apiRuntimeMode)
    {
        _apiRuntimeMode = apiRuntimeMode;
    }

    public IActionResult Index()
    {
        var vm = BuildHomePageViewModel();

        if (!vm.ShowTabScreen)
        {
            vm.Message = "You need to sign-up to view this page";
        }

        return View(vm);
    }

    [HttpGet]
    public IActionResult ComingSoon(string itemType = "Item", string featureAction = "Action", string? id = null)
    {
        return View(new ComingSoonPageViewModel
        {
            ItemType = itemType,
            FeatureAction = featureAction,
            ItemId = id
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private HomePageViewModel BuildHomePageViewModel()
    {
        var isPublicApi = _apiRuntimeMode.IsPublicMode;
        var isCustomApi = _apiRuntimeMode.IsCustomMode;
        var isSignedIn = User?.Identity?.IsAuthenticated ?? false;
        var hasFullAccessRole = User?.Claims.Any(c =>
            c.Type == ClaimTypes.Role && c.Value.Equals("full access", StringComparison.OrdinalIgnoreCase)) ?? false;

        return new HomePageViewModel
        {
            IsPublicApi = isPublicApi,
            IsCustomApi = isCustomApi,
            IsSignedIn = isSignedIn,
            HasFullAccessRole = hasFullAccessRole
        };
    }

}
