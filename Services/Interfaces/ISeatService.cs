using TheatreMs.Api.DTOs.Seat;

namespace TheatreMs.Api.Services.Interfaces;

public interface ISeatService
{
    Task<object> GetTheatreScreensAsync(long theatreId);
    Task<List<SeatDto>> GetScreenSeatsAsync(long theatreId, int screenNumber);
    Task<string> InitializeScreenAsync(long theatreId, int screenNumber, int rows, int seatsPerRow);
    Task UpdateSeatAsync(long seatId, string seatType, double priceMultiplier);
    Task UpdateRowAsync(long theatreId, int screenNumber, string rowName, string seatType, double priceMultiplier);
    Task<string> BulkUpdateAsync(long theatreId, int screenNumber, string selection, string seatType, double priceMultiplier);
    Task<string> DeleteScreenAsync(long theatreId, int screenNumber);
}
