using System.ComponentModel.DataAnnotations;

namespace TheatreMs.Api.Models;

public class PasswordResetToken
{
    public long Id { get; set; }

    [Required, MaxLength(200)]
    public string Token { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public DateTime ExpiresAt { get; set; }

    public bool Used { get; set; } = false;
}
