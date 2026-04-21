namespace TheatreMs.Api.DTOs.Seat;

public class SeatDto
{
    public long? Id { get; set; }
    public string? RowName { get; set; }
    public int? SeatNumber { get; set; }
    public int? ScreenNumber { get; set; }
    public string? SeatType { get; set; }
    public double? PriceMultiplier { get; set; }
}
