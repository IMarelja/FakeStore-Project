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
    public class LoginController : Controller
    {
        private readonly FakeStore.WebApp.Service.IAuthenticationService _service;

        public LoginController(FakeStore.WebApp.Service.IAuthenticationService service)
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

                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(authResponse.token);
                var userId = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                var email = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
                var username = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value
                    ?? loginRequest.username;
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

                if (!string.IsNullOrWhiteSpace(email))
                {
                    claims.Add(new Claim(ClaimTypes.Email, email));
                }

                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = loginRequest.remember_me
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                    authProperties);

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
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
