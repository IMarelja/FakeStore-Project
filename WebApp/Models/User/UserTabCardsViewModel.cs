using FakeStore.ViewModel;

namespace FakeStore.WebApp.Models;

public class UserTabCardsViewModel
{
    public IEnumerable<UserRead> Users { get; set; } = [];
    public bool HasFullAccessRole { get; set; }
    public bool IsPublicApi { get; set; }
}
