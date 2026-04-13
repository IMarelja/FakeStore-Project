namespace FakeStore.WebApp.Service;

public interface IJwtService
{
    int? GetCurrectUserId();
    bool HasFullAccessRole();
}
