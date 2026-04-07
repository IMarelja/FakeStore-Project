using System.Security.Claims;

namespace MyRestApi.Services;

public class ClaimsService(IHttpContextAccessor httpContextAccessor) : IClaimsService
{
    private readonly ClaimsPrincipal _user = httpContextAccessor.HttpContext!.User;

    public int GetUserId() =>
        int.Parse(_user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public string GetUsername() =>
        _user.FindFirstValue(ClaimTypes.Name)!;

    public string GetEmail() =>
        _user.FindFirstValue(ClaimTypes.Email)!;

    public string GetRole() =>
        _user.FindFirstValue(ClaimTypes.Role)!;
}
