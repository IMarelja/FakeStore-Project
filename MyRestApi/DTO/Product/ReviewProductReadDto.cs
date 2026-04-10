using System.Text.Json.Serialization;

namespace MyRestApi.DTO.Product;

public class ReviewProductReadDto
{
    [JsonPropertyName("user_id")]
    public int user_id { get; set; }

    [JsonPropertyName("rating")]
    public int rating { get; set; }

    [JsonPropertyName("comment")]
    public string comment { get; set; } = string.Empty;
}
