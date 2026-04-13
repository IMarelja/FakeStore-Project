using FakeStore.ViewModel;

namespace FakeStore.WebApp.Models;

public class ProductDeletePageViewModel
{
    public ProductRead? Product { get; set; }
    public bool Deleted { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}
