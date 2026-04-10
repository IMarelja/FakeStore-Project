using System.ComponentModel.DataAnnotations;

namespace MyRestApi.DTO.Cart;

public class CartItemAddDto
{
    [Required]
    public int ProductId { get; set; }
    [Required]
    public int Quantity { get; set; }
}
