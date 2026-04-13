using System.ComponentModel.DataAnnotations;

namespace MyRestApi.DTO.Product;

public class ProductCreateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public decimal Price { get; set; }
    [Required(AllowEmptyStrings = true)]
    public string Unit { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = true)]
    public string Image { get; set; } = string.Empty;
    [Required]
    public int Discount { get; set; }
    [Required]
    public bool Available { get; set; }
    [Required]
    public string Brand { get; set; } = string.Empty;
    [Required]
    public string Category { get; set; } = string.Empty;
}
