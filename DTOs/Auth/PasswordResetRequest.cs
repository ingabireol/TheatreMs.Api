using System.ComponentModel.DataAnnotations;

namespace TheatreMs.Api.DTOs.Auth;

public class PasswordResetRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
}
