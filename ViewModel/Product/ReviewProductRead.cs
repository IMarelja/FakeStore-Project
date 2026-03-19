namespace FakeStore.ViewModel;

public class ReviewProductRead
{
    public int user_id { get; set; }

    public int rating { get; set; }

    public string comment { get; set; } = string.Empty;
}
