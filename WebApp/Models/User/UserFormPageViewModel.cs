namespace FakeStore.WebApp.Models;

public class UserFormPageViewModel
{
    public int? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string SubmitLabel { get; set; } = string.Empty;
    public UserFormInputModel Form { get; set; } = new();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}
