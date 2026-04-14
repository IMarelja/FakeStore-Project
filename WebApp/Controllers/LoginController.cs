using FakeStore.ViewModel;
using FakeStore.WebApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FakeStore.WebApp.Controllers
{
    [Authorize(Policy = "CustomApiOnly")]
    public class LoginController : Controller
    {
        private readonly FakeStore.WebApp.Service.IAuthenticationService _service;
        private readonly IJwtService _jwtService;

        public LoginController(FakeStore.WebApp.Service.IAuthenticationService service, IJwtService jwtService)
        {
            _service = service;
            _jwtService = jwtService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Index(LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.username) || string.IsNullOrWhiteSpace(loginRequest.password))
            {
                ViewData["LoginFailedMessage"] = "Username and password are required.";
                return View(loginRequest);
            }

            try
            {
                var authResponse = await _service.login(loginRequest);
                if (string.IsNullOrWhiteSpace(authResponse.token))
                {
                    ViewData["LoginFailedMessage"] = "Login failed. Invalid credentials.";
                    return View(loginRequest);
                }

                await _jwtService.StoreTokenAsync(authResponse.token, loginRequest.remember_me, loginRequest.username);

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                ViewData["LoginFailedMessage"] = "Login failed. Invalid username or password.";
                return View(loginRequest);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _jwtService.DeleteTokenAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
