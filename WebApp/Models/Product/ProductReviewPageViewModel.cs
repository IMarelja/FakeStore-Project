using FakeStore.ViewModel;

namespace FakeStore.WebApp.Models;

public class ProductReviewPageViewModel
{
    public ProductRead? Product { get; set; }
    public ProductReviewFormInputModel Form { get; set; } = new();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}
