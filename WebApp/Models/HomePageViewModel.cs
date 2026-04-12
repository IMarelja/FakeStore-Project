namespace FakeStore.WebApp.Models;

public class HomePageViewModel
{
    public bool IsPublicApi { get; set; }
    public bool IsCustomApi { get; set; }
    public bool IsSignedIn { get; set; }
    public bool HasFullAccessRole { get; set; }
    public string Message { get; set; } = string.Empty;

    public bool ShowTabScreen => IsPublicApi || (IsCustomApi && IsSignedIn);
}
