using FakeStore.ViewModel;

namespace FakeStore.WebApp.Models;

public class UserDeletePageViewModel
{
    public UserRead? User { get; set; }
    public bool Deleted { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}
