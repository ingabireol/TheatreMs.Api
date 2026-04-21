using System.ComponentModel.DataAnnotations;

namespace TheatreMs.Api.DTOs.Auth;

public class OtpVerificationRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required, RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be 6 digits")]
    public string Otp { get; set; } = string.Empty;
}
