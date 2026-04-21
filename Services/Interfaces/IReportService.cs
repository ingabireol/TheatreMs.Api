namespace TheatreMs.Api.Services.Interfaces;

public interface IReportService
{
    Task<object> GetBookingReportAsync(DateTime from, DateTime to);
    Task<object> GetRevenueReportAsync(DateTime from, DateTime to);
    Task<object> GetMoviePerformanceReportAsync(DateTime from, DateTime to);
}
