using System.Security.Claims;

namespace FakeStore.WebApp.Service;

public class JwtService : IJwtService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
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
