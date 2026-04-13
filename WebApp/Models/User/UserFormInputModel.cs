using System.ComponentModel.DataAnnotations;

namespace FakeStore.WebApp.Models;

public class UserFormInputModel
{
    [Required]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
}
