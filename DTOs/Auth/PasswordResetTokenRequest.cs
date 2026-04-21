using System.ComponentModel.DataAnnotations;

namespace TheatreMs.Api.DTOs.Auth;

public class PasswordResetTokenRequest
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [MinLength(6)]
    public string? NewPassword { get; set; }
}
