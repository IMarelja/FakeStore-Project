using System.Text.Json.Serialization;

namespace MyRestApi.DTO;

public sealed class ReviewApiDto
{
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}
