namespace FakeStoreApi.View.Database;

public class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public int OrderStatusId { get; set; }

    public decimal TotalPrice { get; set; }
}
