using System.ComponentModel.DataAnnotations;

namespace MyRestApi.DTO.Cart;

public class CartItemEditDto
{
    [Required]
    public int Quantity { get; set; }
}
