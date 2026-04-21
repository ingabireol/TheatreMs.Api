using System.ComponentModel.DataAnnotations;

namespace TheatreMs.Api.Models;

public class Screening
{
    public long Id { get; set; }

    [Required]
    public long MovieId { get; set; }
    public Movie Movie { get; set; } = null!;

    [Required]
    public long TheatreId { get; set; }
    public Theatre Theatre { get; set; } = null!;

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required]
    public int ScreenNumber { get; set; }

    [Required]
    public ScreeningFormat Format { get; set; } = ScreeningFormat.STANDARD;

    [Required]
    public double BasePrice { get; set; }

    public ICollection<Booking> Bookings { get; set; } = [];
}

public enum ScreeningFormat
{
    STANDARD, IMAX, DOLBY_ATMOS, THREE_D, FOUR_D
}
