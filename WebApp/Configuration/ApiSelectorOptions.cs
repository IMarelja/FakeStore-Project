namespace FakeStore.WebApp.Configuration;

public sealed class ApiSelectorOptions
{
    public const string SectionName = "ApiSelector";

    public string Selected { get; set; } = "Public";
    public ApiEndpointsOptions Apis { get; set; } = new();
}

public sealed class ApiEndpointsOptions
{
    public string Public { get; set; } = string.Empty;
    public string Custom { get; set; } = string.Empty;
}
