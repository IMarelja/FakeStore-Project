namespace MyRestApi.Services;

public interface IClaimsService
{
    int GetUserId();
    string GetUsername();
    string GetEmail();
    string GetRole();
}
