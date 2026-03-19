namespace FakeStore.View;

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

    public string Brand { get; set; } = string.Empty;

    public double Rating { get; set; } 

    public List<Review> Reviews { get; set; } = []; 
}
