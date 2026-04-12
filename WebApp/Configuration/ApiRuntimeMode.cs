namespace FakeStore.WebApp.Configuration;

public sealed class ApiRuntimeMode
{
    public ApiRuntimeMode(string selected, bool isPublicMode, bool isCustomMode)
    {
        Selected = selected;
        IsPublicMode = isPublicMode;
        IsCustomMode = isCustomMode;
    }

    public string Selected { get; }
    public bool IsPublicMode { get; }
    public bool IsCustomMode { get; }
}
