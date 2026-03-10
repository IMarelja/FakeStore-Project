namespace FakeStore.ViewModel;

public class OrderRead
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public string Status { get; set; } = string.Empty;

    public double TotalPrice { get; set; }

    public List<ItemCartRead> Items { get; set; } = [];
}
