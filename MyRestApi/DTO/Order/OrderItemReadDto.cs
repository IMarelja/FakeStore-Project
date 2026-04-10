using System.Text.Json.Serialization;

namespace MyRestApi.DTO.Order;

public class OrderItemReadDto
{
    [JsonPropertyName("product_id")]
    public int product_id { get; set; }

    [JsonPropertyName("quantity")]
    public int quantity { get; set; }
}
