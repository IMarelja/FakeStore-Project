using System.Text.Json.Serialization;

namespace FakeStore.ViewModel;

public class AuthenticationResponse
{
    [JsonPropertyName("token")]
    public string token { get; set; } = string.Empty;
}
