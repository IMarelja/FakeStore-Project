namespace FakeStore.ViewModel;

public class LoginRequest
{
    public string username { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
    public bool remember_me { get; set; } = false;
}
