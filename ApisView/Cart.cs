namespace FakeStore.View;

public class Cart
{
    public int CartId { get; set; }

    public int UserId { get; set; }

    public List<CartItem> Items { get; set; } = []; 
}
