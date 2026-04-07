using System.Text.Json.Serialization;

namespace FakeStore.ViewModel;

public class ItemOrderRead
{
    [JsonPropertyName("product_id")]
    public int product_id { get; set; }

    [JsonPropertyName("quantity")]
    public int quantity { get; set; }
}
