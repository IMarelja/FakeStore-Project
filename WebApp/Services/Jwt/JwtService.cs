using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FakeStore.WebApp.Service;

public class JwtService : IJwtService
{
    private const string AccessTokenClaimType = "access_token";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task StoreTokenAsync(string token, bool isPersistent, string? fallbackUsername = null)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("JWT token is required.");

        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("No active HTTP context available.");

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var userId = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        var email = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
        var username = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value
            ?? fallbackUsername
            ?? throw new InvalidOperationException("JWT token does not contain a username.");
        var roles = jwt.Claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type == "role" || c.Type == "roles")
            .Select(c => c.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new(AccessTokenClaimType, token)
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
            IsPersistent = isPersistent
        };

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
            authProperties);
    }

    public string? GetAccessToken()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirstValue(AccessTokenClaimType);
    }

    public int? GetCurrectUserId()
    {
        var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : null;
    }

    public bool HasFullAccessRole()
    {
        return _httpContextAccessor.HttpContext?.User.Claims.Any(claim =>
            claim.Type == ClaimTypes.Role
            && claim.Value.Equals("full access", StringComparison.OrdinalIgnoreCase)) ?? false;
    }
}
