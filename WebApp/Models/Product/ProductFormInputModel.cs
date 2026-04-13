using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FakeStore.WebApp.Models;

public class ProductFormInputModel
{
    [Required]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Required]
    [Display(Name = "Unit")]
    public string Unit { get; set; } = string.Empty;

    [Display(Name = "Image URL")]
    public string? ImageUrl { get; set; }

    [Display(Name = "Upload image")]
    public IFormFile? ImageFile { get; set; }

    public string? ExistingImage { get; set; }

    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
    [Display(Name = "Discount (%)")]
    public int Discount { get; set; }

    [Display(Name = "Available")]
    public bool Available { get; set; }

    [Required]
    [Display(Name = "Brand")]
    public string Brand { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Category")]
    public string Category { get; set; } = string.Empty;
}
