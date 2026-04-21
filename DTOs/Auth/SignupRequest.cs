using System.ComponentModel.DataAnnotations;

namespace TheatreMs.Api.DTOs.Auth;

public class SignupRequest
{
    [Required, MinLength(3), MaxLength(20)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6), MaxLength(40)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }
}
