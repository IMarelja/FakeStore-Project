namespace FakeStore.ViewModel;

public class CartRead
{
    public int cart_id { get; set; }

    public int user_id { get; set; }

    public List<ItemCartRead> items { get; set; } = []; 
}
