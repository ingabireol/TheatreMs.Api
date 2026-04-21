using System.ComponentModel.DataAnnotations;

namespace TheatreMs.Api.DTOs.Theatre;

public class TheatreDto
{
    public long? Id { get; set; }

    [MaxLength(100)]
    public string? Name { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public int? TotalScreens { get; set; }
    public string? ImageUrl { get; set; }
}
