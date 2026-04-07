using System.Text.Json.Serialization;

namespace FakeStore.ViewModel;

public class ReviewProductRead
{
    [JsonPropertyName("user_id")]
    public int user_id { get; set; }

    [JsonPropertyName("rating")]
    public int rating { get; set; }

    [JsonPropertyName("comment")]
    public string comment { get; set; } = string.Empty;
}
