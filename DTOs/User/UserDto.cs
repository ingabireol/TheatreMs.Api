using System.ComponentModel.DataAnnotations;
using TheatreMs.Api.Models;

namespace TheatreMs.Api.DTOs.User;

public class UserDto
{
    public long? Id { get; set; }

    [MinLength(3), MaxLength(20)]
    public string? Username { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [MinLength(6), MaxLength(40)]
    public string? Password { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public UserRole? Role { get; set; }
}
