using System.ComponentModel.DataAnnotations;

namespace TheatreMs.Api.Models;

public class Theatre
{
    public long Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public int? TotalScreens { get; set; }

    public string? ImageUrl { get; set; }

    public ICollection<Screening> Screenings { get; set; } = [];
    public ICollection<Seat> Seats { get; set; } = [];
}
