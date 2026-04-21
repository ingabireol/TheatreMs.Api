using TheatreMs.Api.Models;

namespace TheatreMs.Api.DTOs.Screening;

public class ScreeningDto
{
    public long? Id { get; set; }
    public long? MovieId { get; set; }
    public string? MovieTitle { get; set; }
    public long? TheatreId { get; set; }
    public string? TheatreName { get; set; }
    public DateTime? StartTime { get; set; }
    public string? StartDateString { get; set; }
    public string? StartTimeString { get; set; }
    public DateTime? EndTime { get; set; }
    public int? ScreenNumber { get; set; }
    public ScreeningFormat? Format { get; set; }
    public double? BasePrice { get; set; }
}
