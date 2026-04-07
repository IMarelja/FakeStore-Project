using System.Text.Json.Serialization;

namespace FakeStore.ViewModel;

public class OrderRead
{
    [JsonPropertyName("order_id")]
    public int order_id { get; set; }

    [JsonPropertyName("user_id")]
    public int user_id { get; set; }

    [JsonPropertyName("status")]
    public string status { get; set; } = string.Empty;

    [JsonPropertyName("total_price")]
    public double total_price { get; set; }

    [JsonPropertyName("items")]
    public List<ItemOrderRead> items { get; set; } = [];
}
