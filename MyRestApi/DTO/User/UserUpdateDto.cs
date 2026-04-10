using System.ComponentModel.DataAnnotations;

namespace MyRestApi.DTO.User;

public class UserUpdateDto
{
    [Required]
    public string? Username { get; set; }
    [Required]
    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
}
