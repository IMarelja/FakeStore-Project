using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FakeStore.WebApp.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IConfiguration _configuration;

    public ProfileController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    
    public IActionResult Index()
    {
        if (!IsCustomApiMode())
        {
            return NotFound();
        }

        return View();
    }

    private bool IsCustomApiMode()
    {
        var selectedApi = _configuration["ApiSelector:Selected"] ?? "Public";
        return selectedApi.Equals("Custom", StringComparison.OrdinalIgnoreCase);
    }
}
