using FakeStore.ViewModel;
using FakeStore.WebApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FakeStore.WebApp.Controllers
{
    [Authorize(Policy = "CustomApiOnly")]
    public class RegisterController : Controller
    {
        private readonly FakeStore.WebApp.Service.IAuthenticationService _service;

        public RegisterController(FakeStore.WebApp.Service.IAuthenticationService service)
        {
            _service = service;
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

                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(authResponse.token);
                var userId = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                var email = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
                var username = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value
                    ?? registerRequest.username;
                var roles = jwt.Claims
                    .Where(c => c.Type == ClaimTypes.Role || c.Type == "role" || c.Type == "roles")
                    .Select(c => c.Value)
                    .Distinct()
                    .ToList();

                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, username),
                    new("access_token", authResponse.token)
                };

                if (!string.IsNullOrWhiteSpace(userId))
                {
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
                }

                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = registerRequest.remember_me
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                    authProperties);

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
