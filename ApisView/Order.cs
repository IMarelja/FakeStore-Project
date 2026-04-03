namespace FakeStore.View;

public class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public string OrderStatus { get; set; } = string.Empty;

    public decimal TotalPrice { get; set; }

    public List<OrderItem> Items { get; set; } = []; 
}
