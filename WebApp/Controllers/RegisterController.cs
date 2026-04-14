using FakeStore.ViewModel;
using FakeStore.WebApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FakeStore.WebApp.Controllers
{
    [Authorize(Policy = "CustomApiOnly")]
    public class RegisterController : Controller
    {
        private readonly FakeStore.WebApp.Service.IAuthenticationService _service;
        private readonly IJwtService _jwtService;

        public RegisterController(FakeStore.WebApp.Service.IAuthenticationService service, IJwtService jwtService)
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

            return View(new RegisterRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Index(RegisterRequest registerRequest)
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrWhiteSpace(registerRequest.username)
                || string.IsNullOrWhiteSpace(registerRequest.email)
                || string.IsNullOrWhiteSpace(registerRequest.password))
            {
                ViewData["RegisterFailedMessage"] = "Username, email, and password are required.";
                return View(registerRequest);
            }

            try
            {
                var authResponse = await _service.register(registerRequest);
                if (string.IsNullOrWhiteSpace(authResponse.token))
                {
                    ViewData["RegisterFailedMessage"] = "Registration failed.";
                    return View(registerRequest);
                }

                await _jwtService.StoreTokenAsync(authResponse.token, registerRequest.remember_me, registerRequest.username);

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                ViewData["RegisterFailedMessage"] = "Registration failed. Check your input and try again.";
                return View(registerRequest);
            }
        }
    }
}
