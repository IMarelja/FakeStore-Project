using System.Text.Json.Serialization;

namespace FakeStore.ViewModel;

public class ProductRead
{
    [JsonPropertyName("product_id")]
    public int product_id { get; set; }

    [JsonPropertyName("name")]
    public string name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string description { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public double price { get; set; }

    [JsonPropertyName("unit")]
    public string unit { get; set; } = string.Empty;

    [JsonPropertyName("image")]
    public string image { get; set; } = string.Empty;

    [JsonPropertyName("discount")]
    public int discount { get; set; }

    [JsonPropertyName("availability")]
    public bool availability { get; set; }

    [JsonPropertyName("brand")]
    public string brand { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string category { get; set; } = string.Empty;
    
    [JsonPropertyName("rating")]
    public double rating { get; set; }

    [JsonPropertyName("reviews")]
    public List<ReviewProductRead> reviews { get; set; } = [];
}
