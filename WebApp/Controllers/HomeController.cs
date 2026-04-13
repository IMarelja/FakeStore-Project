using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FakeStore.WebApp.Models;
using FakeStore.WebApp.Configuration;
using FakeStore.WebApp.Service;

namespace FakeStore.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ApiRuntimeMode _apiRuntimeMode;
    private readonly IJwtService _jwtService;

    public HomeController(
        ApiRuntimeMode apiRuntimeMode,
        IJwtService jwtService)
    {
        _apiRuntimeMode = apiRuntimeMode;
        _jwtService = jwtService;
    }

    public IActionResult Index(string? tab = null)
    {
        var vm = BuildHomePageViewModel(tab);

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

    private HomePageViewModel BuildHomePageViewModel(string? requestedTab)
    {
        var isPublicApi = _apiRuntimeMode.IsPublicMode;
        var isCustomApi = _apiRuntimeMode.IsCustomMode;
        var isSignedIn = User?.Identity?.IsAuthenticated ?? false;
        var hasFullAccessRole = _jwtService.HasFullAccessRole();
        var normalizedTab = NormalizeTabKey(requestedTab);

        return new HomePageViewModel
        {
            IsPublicApi = isPublicApi,
            IsCustomApi = isCustomApi,
            IsSignedIn = isSignedIn,
            HasFullAccessRole = hasFullAccessRole,
            ActiveTab = normalizedTab
        };
    }

    private static string NormalizeTabKey(string? tab)
    {
        if (string.IsNullOrWhiteSpace(tab))
        {
            return "products";
        }

        return tab.Trim().ToLowerInvariant() switch
        {
            "products" => "products",
            "users" => "users",
            "cart" => "cart",
            "orders" => "orders",
            _ => "products"
        };
    }
}
