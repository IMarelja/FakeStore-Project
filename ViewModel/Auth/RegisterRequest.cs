using System.Text.Json.Serialization;

namespace FakeStore.ViewModel;

public class RegisterRequest
{
    [JsonPropertyName("username")]
    public string username { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string email { get; set; } = string.Empty;
    
    [JsonPropertyName("password")]
    public string password { get; set; } = string.Empty;

    [JsonPropertyName("remember_me")]
    public bool remember_me { get; set; } = false;
}
