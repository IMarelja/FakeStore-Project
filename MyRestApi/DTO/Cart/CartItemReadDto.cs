using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyRestApi.DTO.Cart;

public class CartItemReadDto
{
    [JsonPropertyName("product_id")]
    public int product_id { get; set; }

    [JsonPropertyName("quantity")]
    public int quantity { get; set; }
}
