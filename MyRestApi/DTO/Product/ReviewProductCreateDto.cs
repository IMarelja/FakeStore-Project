using System.ComponentModel.DataAnnotations;

namespace MyRestApi.DTO.Product;

public class ReviewProductCreateDto
{
    [Required]
    public string Comment { get; set; } = string.Empty;
    [Required]
    [Length(1, 5)]
    public int Rating { get; set; }
}
