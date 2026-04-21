using System.ComponentModel.DataAnnotations;

namespace TheatreMs.Api.Models;

public class Booking
{
    public long Id { get; set; }

    [Required]
    public long UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    public long ScreeningId { get; set; }
    public Screening Screening { get; set; } = null!;

    [Required, MaxLength(50)]
    public string BookingNumber { get; set; } = string.Empty;

    [Required]
    public DateTime BookingTime { get; set; }

    [Required]
    public double TotalAmount { get; set; }

    [Required]
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.PENDING;

    public string? PaymentMethod { get; set; }

    // Stored as JSON array in SQL Server
    public List<string> BookedSeats { get; set; } = [];
}

public enum PaymentStatus
{
    PENDING, COMPLETED, CANCELLED, REFUNDED
}
