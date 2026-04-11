using Microsoft.AspNetCore.Mvc;

namespace FakeStore.WebApp.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IConfiguration _configuration;

        public RegisterController(IConfiguration configuration)
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
}
