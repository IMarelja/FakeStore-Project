using System.ComponentModel.DataAnnotations;

namespace MyRestApi.DTO.Order;

public class OrderUpdateDto
{
    [Required]
    public string? Status { get; set; }
}
