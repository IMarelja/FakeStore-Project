using System.Text.Json.Serialization;

namespace DataSeeder.Models;

public sealed class OrderApiModel
{
    [JsonPropertyName("order_id")]     public int     OrderId     { get; set; }
    [JsonPropertyName("user_id")]      public int     UserId      { get; set; }
    [JsonPropertyName("status")]       public string  Status      { get; set; } = string.Empty;
    [JsonPropertyName("total_price")]  public decimal TotalPrice  { get; set; }
    [JsonPropertyName("items")]        public List<OrderItemApiModel> Items { get; set; } = [];
}

public sealed class OrderItemApiModel
{
    [JsonPropertyName("product_id")] public int ProductId { get; set; }
    [JsonPropertyName("quantity")]   public int Quantity  { get; set; }
}
