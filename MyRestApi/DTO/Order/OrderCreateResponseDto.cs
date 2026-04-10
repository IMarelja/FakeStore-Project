using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyRestApi.DTO.Order;

public class OrderCreateResponseDto
{
    [JsonPropertyName("order_id")]
    public int OrderId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
