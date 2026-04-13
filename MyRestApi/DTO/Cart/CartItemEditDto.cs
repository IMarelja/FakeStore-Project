using System.ComponentModel.DataAnnotations;

namespace MyRestApi.DTO.Cart;

public class CartItemEditDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
