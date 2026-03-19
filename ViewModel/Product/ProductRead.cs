namespace FakeStore.ViewModel;

public class ProductRead
{
    public int product_id { get; set; }

    public string name { get; set; } = string.Empty;

    public string description { get; set; } = string.Empty;

    public double price { get; set; }

    public string unit { get; set; } = string.Empty;

    public string image { get; set; } = string.Empty;

    public int discount { get; set; }

    public bool availability { get; set; }

    public string brand { get; set; } = string.Empty;

    public string category { get; set; } = string.Empty;
    public double rating { get; set; }

    public List<ReviewProductRead> reviews { get; set; } = [];
}
