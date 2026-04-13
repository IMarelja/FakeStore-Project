using FakeStore.ViewModel;

namespace FakeStore.WebApp.Models;

public class CartTabCardsViewModel
{
    public IEnumerable<CartRead> Carts { get; set; } = [];
    public int? CurrentUserId { get; set; }
    public bool HasFullAccessRole { get; set; }
    public bool IsPublicApi { get; set; }
}
