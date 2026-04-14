namespace FakeStore.ViewModel;

public class ProductUpdate
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? Image { get; set; }
    public int Discount { get; set; }
    public bool Available { get; set; }
    public string Brand { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Category { get; set; } = string.Empty;
}
