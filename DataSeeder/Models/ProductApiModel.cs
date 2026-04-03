using System.Text.Json.Serialization;

namespace DataSeeder.Models;

public sealed class ProductApiModel
{
    [JsonPropertyName("product_id")]   public int     ProductId   { get; set; }
    [JsonPropertyName("name")]         public string  Name        { get; set; } = string.Empty;
    [JsonPropertyName("description")]  public string  Description { get; set; } = string.Empty;
    [JsonPropertyName("price")]        public decimal Price       { get; set; }
    [JsonPropertyName("unit")]         public string  Unit        { get; set; } = string.Empty;
    [JsonPropertyName("image")]        public string  Image       { get; set; } = string.Empty;
    [JsonPropertyName("discount")]     public int     Discount    { get; set; }
    [JsonPropertyName("availability")] public bool    Availability { get; set; }
    [JsonPropertyName("brand")]        public string  Brand       { get; set; } = string.Empty;
    [JsonPropertyName("category")]     public string  Category    { get; set; } = string.Empty;
    [JsonPropertyName("rating")]       public double  Rating      { get; set; }
    [JsonPropertyName("reviews")]      public List<ReviewApiModel> Reviews { get; set; } = [];
}

public sealed class ReviewApiModel
{
    [JsonPropertyName("user_id")] public int    UserId  { get; set; }
    [JsonPropertyName("rating")]  public int    Rating  { get; set; }
    [JsonPropertyName("comment")] public string Comment { get; set; } = string.Empty;
}
