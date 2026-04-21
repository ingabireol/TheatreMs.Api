using System.ComponentModel.DataAnnotations;

namespace TheatreMs.Api.Models;

public class Seat
{
    public long Id { get; set; }

    [Required]
    public long TheatreId { get; set; }
    public Theatre Theatre { get; set; } = null!;

    [Required]
    public int ScreenNumber { get; set; }

    [Required, MaxLength(10)]
    public string RowName { get; set; } = string.Empty;

    [Required]
    public int SeatNumber { get; set; }

    [Required]
    public SeatType SeatType { get; set; } = SeatType.STANDARD;

    [Required]
    public double PriceMultiplier { get; set; } = 1.0;
}

public enum SeatType
{
    STANDARD, PREMIUM, VIP, ACCESSIBLE
}
