using TheatreMs.Api.DTOs.Movie;
using TheatreMs.Api.DTOs.Screening;

namespace TheatreMs.Api.Services.Interfaces;

public interface IMovieService
{
    Task<List<MovieDto>> GetAllAsync(string? query, string? genre, DateOnly? date);
    Task<MovieDto?> GetByIdAsync(long id);
    Task<List<MovieDto>> SearchAsync(string query);
    Task<List<MovieDto>> GetByGenreAsync(string genre);
    Task<List<string>> GetGenresAsync();
    Task<List<string>> GetRatingsAsync();
    Task<Dictionary<DateOnly, List<ScreeningDto>>> GetScreeningsForMovieAsync(long movieId, int days);
    Task<Dictionary<long, List<ScreeningDto>>> GetMoviesWithScreeningsAsync(DateOnly? date);

    // Admin / paginated
    Task<(List<MovieDto> Items, int TotalCount)> GetPagedAsync(string? search, string? genre, string sortBy, string sortOrder, int page, int size);
    Task<MovieDto> CreateAsync(MovieDto dto);
    Task<MovieDto> UpdateAsync(long id, MovieDto dto);
    Task DeleteAsync(long id);
    Task<object> GetMovieScreeningsAdminAsync(long movieId);
}
