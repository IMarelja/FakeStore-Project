using FakeStore.ViewModel;

namespace FakeStore.WebApp.Models;

public class OrdersPageViewModel
{
    public IEnumerable<OrderRead> Orders { get; set; } = [];
    public string? ErrorMessage { get; set; }
}
