using FakeStore.ViewModel;

namespace FakeStore.WebApp.Models;

public class ProductTabCardsViewModel
{
    public IEnumerable<ProductRead> Products { get; set; } = [];
    public bool HasFullAccessRole { get; set; }
}
