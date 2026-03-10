namespace FakeStore.ViewModel;

public class ReviewProductRead
{
    public int UserId { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;
}
