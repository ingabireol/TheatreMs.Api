using System.ComponentModel.DataAnnotations;

namespace TheatreMs.Api.Models;

public class User
{
    public long Id { get; set; }

    [Required, MaxLength(20)]
    public string Username { get; set; } = string.Empty;

    [Required, MaxLength(50), EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(120)]
    public string Password { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [Required]
    public UserRole Role { get; set; } = UserRole.ROLE_USER;

    public ICollection<Booking> Bookings { get; set; } = [];
}

public enum UserRole
{
    ROLE_USER,
    ROLE_MANAGER,
    ROLE_ADMIN
}
