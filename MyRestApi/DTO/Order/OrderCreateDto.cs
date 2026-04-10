using System.ComponentModel.DataAnnotations;

namespace MyRestApi.DTO.Order;

public class OrderCreateDto
{
    [Required]
    public int UserId { get; set; }
    [Required]
    public List<OrderItemReadDto> Items { get; set; } = [];
}
