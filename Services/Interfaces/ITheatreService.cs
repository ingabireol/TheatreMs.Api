using TheatreMs.Api.DTOs.Theatre;

namespace TheatreMs.Api.Services.Interfaces;

public interface ITheatreService
{
    Task<List<TheatreDto>> GetAllAsync(string? search, string sortBy);
    Task<TheatreDto?> GetByIdAsync(long id);
    Task<TheatreDto> CreateAsync(TheatreDto dto);
    Task<TheatreDto> UpdateAsync(long id, TheatreDto dto);
    Task DeleteAsync(long id);
    Task<object> GetTheatreSeatsAsync(long id);
    Task InitializeSeatsAsync(long theatreId, int screenNumber, int rows, int seatsPerRow);
    Task<object> GetTheatreScreeningsAsync(long id);
}
