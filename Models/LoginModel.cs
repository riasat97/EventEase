using System.ComponentModel.DataAnnotations;

namespace EventEase.Models;

public class LoginModel
{
    [Required(ErrorMessage = "Username or email is required")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}