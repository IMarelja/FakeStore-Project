using System.ComponentModel.DataAnnotations;

namespace MyRestApi.DTO.Cart;

public class CartItemAddDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
