namespace FakeStore.ViewModel;

public class RegisterRequest
{
    public string username { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
    public bool remember_me { get; set; } = false;
}
