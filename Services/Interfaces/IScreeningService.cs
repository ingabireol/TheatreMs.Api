using TheatreMs.Api.DTOs.Screening;

namespace TheatreMs.Api.Services.Interfaces;

public interface IScreeningService
{
    Task<List<ScreeningDto>> GetAllAsync(long? movieId, long? theatreId, DateOnly? date);
    Task<ScreeningDto?> GetByIdAsync(long id);
    Task<List<ScreeningDto>> GetByMovieAsync(long movieId, int days);
    Task<List<ScreeningDto>> GetByTheatreAsync(long theatreId, DateOnly? date);
    Task<Dictionary<string, List<ScreeningDto>>> GetUpcomingAsync(int days);
    Task<List<ScreeningDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<HashSet<string>> GetAvailableSeatsAsync(long screeningId);
    Task<HashSet<string>> GetBookedSeatsAsync(long screeningId);
    Task<object> GetSeatLayoutAsync(long screeningId);

    // Admin
    Task<(List<ScreeningDto> Items, int TotalCount)> GetPagedAsync(long? movieId, long? theatreId, DateOnly? date, string? search, string sortBy, string sortOrder, int page, int size);
    Task<List<string>> GetFormatsAsync();
    Task<object> GetScreeningAdminDetailAsync(long id);
    Task<ScreeningDto> CreateAsync(ScreeningDto dto);
    Task<ScreeningDto> UpdateAsync(long id, ScreeningDto dto);
    Task DeleteAsync(long id);
    Task<List<object>> GetScreeningBookingsAsync(long id);
}
