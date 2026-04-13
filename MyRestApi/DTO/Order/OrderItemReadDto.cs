using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace MyRestApi.DTO.Order;

public class OrderItemReadDto
{
    [JsonPropertyName("product_id")]
    [Range(1, int.MaxValue)]
    public int product_id { get; set; }

    [JsonPropertyName("quantity")]
    [Range(1, int.MaxValue)]
    public int quantity { get; set; }
}
