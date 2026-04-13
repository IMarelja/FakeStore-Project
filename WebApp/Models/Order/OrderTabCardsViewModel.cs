using FakeStore.ViewModel;

namespace FakeStore.WebApp.Models;

public class OrderTabCardsViewModel
{
    public IEnumerable<OrderRead> Orders { get; set; } = [];
    public int? CurrentUserId { get; set; }
    public bool HasFullAccessRole { get; set; }
}
