namespace FakeStore.WebApp.Models;

public class ProductFormPageViewModel
{
    public int? ProductId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string SubmitLabel { get; set; } = string.Empty;
    public ProductFormInputModel Form { get; set; } = new();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}
