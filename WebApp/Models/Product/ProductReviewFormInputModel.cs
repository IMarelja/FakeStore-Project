using System.ComponentModel.DataAnnotations;

namespace FakeStore.WebApp.Models;

public class ProductReviewFormInputModel
{
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    [Display(Name = "Rating")]
    public int Rating { get; set; } = 5;

    [Required]
    [StringLength(1000, ErrorMessage = "Comment is too long.")]
    [Display(Name = "Comment")]
    public string Comment { get; set; } = string.Empty;
}
