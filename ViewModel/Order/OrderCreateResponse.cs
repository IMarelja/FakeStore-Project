namespace FakeStore.ViewModel;

public class OrderCreateResponse
{
    public int OrderId { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
