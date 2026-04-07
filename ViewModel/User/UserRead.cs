using System.Text.Json.Serialization;

namespace FakeStore.ViewModel;

public class UserRead
{
    [JsonPropertyName("user_id")]
    public int user_id { get; set; }

    [JsonPropertyName("username")]
    public string username { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string email { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string password { get; set; } = string.Empty;
}
