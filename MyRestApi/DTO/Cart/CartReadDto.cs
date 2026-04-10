using System.Text.Json.Serialization;

namespace MyRestApi.DTO.Cart;

public class CartReadDto
{
    [JsonPropertyName("cart_id")]
    public int cart_id { get; set; }

    [JsonPropertyName("user_id")]
    public int user_id { get; set; }

    [JsonPropertyName("items")]
    public List<CartItemReadDto> items { get; set; } = [];
}
