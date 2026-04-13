using System.Text.Json.Serialization;

namespace MyRestApi.DTO.Cart;

public class CartItemReadDto
{
    [JsonPropertyName("cart_item_id")]
    public int cart_item_id { get; set; }

    [JsonPropertyName("product_id")]
    public int product_id { get; set; }

    [JsonPropertyName("quantity")]
    public int quantity { get; set; }
}
