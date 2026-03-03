namespace FakeStore.ViewModel;

public class OrderCreate
{
    public int UserId { get; set; }

    public List<ItemCartRead> Items { get; set; } = [];
}
