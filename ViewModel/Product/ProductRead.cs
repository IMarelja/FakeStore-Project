namespace FakeStore.ViewModel;

public class ProductRead
{
    public int ProductId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public double Price { get; set; }

    public string Unit { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    public int Discount { get; set; }

    public bool Available { get; set; }

    public string Brand { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public List<ReviewProductRead> Reviews { get; set; } = [];
}
