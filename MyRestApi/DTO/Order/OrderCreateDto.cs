using System.ComponentModel.DataAnnotations;

namespace MyRestApi.DTO.Order;

public class OrderCreateDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int UserId { get; set; }

    [Required]
    [MinLength(1)]
    public List<OrderItemReadDto> Items { get; set; } = [];
}
