using FakeStore.ViewModel;

namespace FakeStore.WebApp.Models;

public class ProfilePageViewModel
{
    public UserRead? CurrentUser { get; set; }
    public UserUpdate UpdateRequest { get; set; } = new();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}
