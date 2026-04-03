using System.Text.Json.Serialization;

namespace MyRestApi.DTO;

public sealed class ReviewApiDto
{
    [JsonPropertyName("user_id")]  public int UserId { get; set; }
    [JsonPropertyName("rating")]   public int Rating { get; set; }
    [JsonPropertyName("comment")]  public string Comment { get; set; } = string.Empty;
}
