namespace FakeStoreApi.View.Database;

public class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Unit { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    public int Discount { get; set; }

    public bool Available { get; set; }

    public int BrandId { get; set; }

    public int CategoryId { get; set; }
}
