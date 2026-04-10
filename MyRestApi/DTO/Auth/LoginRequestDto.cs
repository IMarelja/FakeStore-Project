using System.ComponentModel.DataAnnotations;

namespace MyRestApi.DTO.Auth;

public class LoginRequestDto
{
    [Required]
    public string username { get; set; } = string.Empty;
    [Required]
    public string password { get; set; } = string.Empty;
    public bool remember_me { get; set; } = false;
}
