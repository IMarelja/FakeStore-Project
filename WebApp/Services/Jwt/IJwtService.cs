namespace FakeStore.WebApp.Service;

public interface IJwtService
{
    Task StoreTokenAsync(string token, bool isPersistent, string? fallbackUsername = null);
    string? GetAccessToken();
    int? GetCurrectUserId();
    bool HasFullAccessRole();
}
