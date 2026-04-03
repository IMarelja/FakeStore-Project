using System.Text.Json.Serialization;

namespace DataSeeder.Models;

public sealed class CartApiModel
{
    [JsonPropertyName("cart_id")] public int    CartId { get; set; }
    [JsonPropertyName("user_id")] public int    UserId { get; set; }
    [JsonPropertyName("items")]   public List<CartItemApiModel> Items { get; set; } = [];
}

public sealed class CartItemApiModel
{
    [JsonPropertyName("product_id")] public int ProductId { get; set; }
    [JsonPropertyName("quantity")]   public int Quantity  { get; set; }
}
