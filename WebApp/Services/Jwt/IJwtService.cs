namespace FakeStore.WebApp.Service;

public interface IJwtService
{
    Task StoreTokenAsync(string token, bool isPersistent, string? fallbackUsername = null);
    Task DeleteTokenAsync();
    string? GetAccessToken();
    int? GetCurrectUserId();
    bool HasFullAccessRole();
}
