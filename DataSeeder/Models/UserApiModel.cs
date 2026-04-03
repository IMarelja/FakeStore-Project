using System.Text.Json.Serialization;

namespace DataSeeder.Models;

public sealed class UserApiModel
{
    [JsonPropertyName("user_id")]  public int    UserId   { get; set; }
    [JsonPropertyName("username")] public string Username { get; set; } = string.Empty;
    [JsonPropertyName("email")]    public string Email    { get; set; } = string.Empty;
    [JsonPropertyName("password")] public string Password { get; set; } = string.Empty;
    [JsonPropertyName("role")]     public string Role     { get; set; } = string.Empty;
}
