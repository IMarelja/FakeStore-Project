using System.Text.Json.Serialization;

namespace FakeStore.ViewModel;

public class CartRead
{
    [JsonPropertyName("cart_id")]
    public int cart_id { get; set; }

    [JsonPropertyName("user_id")]
    public int user_id { get; set; }

    [JsonPropertyName("items")]
    public List<ItemCartRead> items { get; set; } = []; 
}
