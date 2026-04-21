using Microsoft.EntityFrameworkCore;
using TheatreMs.Api.Data;
using TheatreMs.Api.DTOs.Movie;
using TheatreMs.Api.DTOs.Screening;
using TheatreMs.Api.Models;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Services.Implementations;

public class MovieService(AppDbContext db) : IMovieService
{
    public async Task<List<MovieDto>> GetAllAsync(string? query, string? genre, DateOnly? date)
    {
        var q = db.Movies.AsQueryable();
        if (!string.IsNullOrEmpty(query))
            q = q.Where(m => m.Title.Contains(query) || (m.Director != null && m.Director.Contains(query)));
        if (!string.IsNullOrEmpty(genre) && Enum.TryParse<MovieGenre>(genre, true, out var g))
            q = q.Where(m => m.Genre == g);
        if (date.HasValue)
            q = q.Where(m => m.Screenings.Any(s => DateOnly.FromDateTime(s.StartTime) == date.Value));
        var entities = await q.ToListAsync();
        return entities.Select(MapToDto).ToList();
    }

    public async Task<MovieDto?> GetByIdAsync(long id)
    {
        var m = await db.Movies.FindAsync(id);
        return m == null ? null : MapToDto(m);
    }

    public async Task<List<MovieDto>> SearchAsync(string query)
    {
        var entities = await db.Movies
            .Where(m => m.Title.Contains(query) || (m.Director != null && m.Director.Contains(query)) || (m.Cast != null && m.Cast.Contains(query)))
            .ToListAsync();
        return entities.Select(MapToDto).ToList();
    }

    public async Task<List<MovieDto>> GetByGenreAsync(string genre)
    {
        if (!Enum.TryParse<MovieGenre>(genre, true, out var g))
            return [];
        var entities = await db.Movies.Where(m => m.Genre == g).ToListAsync();
        return entities.Select(MapToDto).ToList();
    }

    public Task<List<string>> GetGenresAsync() =>
        Task.FromResult(Enum.GetNames<MovieGenre>().ToList());

    public Task<List<string>> GetRatingsAsync() =>
        Task.FromResult(Enum.GetNames<MovieRating>().ToList());

    public async Task<Dictionary<DateOnly, List<ScreeningDto>>> GetScreeningsForMovieAsync(long movieId, int days)
    {
        var until = DateTime.UtcNow.AddDays(days);
        var screenings = await db.Screenings
            .Include(s => s.Theatre)
            .Where(s => s.MovieId == movieId && s.StartTime >= DateTime.UtcNow && s.StartTime <= until)
            .ToListAsync();

        return screenings
            .GroupBy(s => DateOnly.FromDateTime(s.StartTime))
            .ToDictionary(g => g.Key, g => g.Select(s => MapScreeningToDto(s)).ToList());
    }

    public async Task<Dictionary<long, List<ScreeningDto>>> GetMoviesWithScreeningsAsync(DateOnly? date)
    {
        var q = db.Screenings.Include(s => s.Movie).Include(s => s.Theatre).AsQueryable();
        if (date.HasValue)
            q = q.Where(s => DateOnly.FromDateTime(s.StartTime) == date.Value);
        else
            q = q.Where(s => s.StartTime >= DateTime.UtcNow);

        var screenings = await q.ToListAsync();
        return screenings
            .GroupBy(s => s.MovieId)
            .ToDictionary(g => g.Key, g => g.Select(s => MapScreeningToDto(s)).ToList());
    }

    public async Task<(List<MovieDto> Items, int TotalCount)> GetPagedAsync(string? search, string? genre, string sortBy, string sortOrder, int page, int size)
    {
        var q = db.Movies.AsQueryable();
        if (!string.IsNullOrEmpty(search))
            q = q.Where(m => m.Title.Contains(search) || (m.Director != null && m.Director.Contains(search)));
        if (!string.IsNullOrEmpty(genre) && Enum.TryParse<MovieGenre>(genre, true, out var g))
            q = q.Where(m => m.Genre == g);

        q = (sortBy.ToLower(), sortOrder.ToLower()) switch
        {
            ("title", "desc") => q.OrderByDescending(m => m.Title),
            ("releasedate", "asc") => q.OrderBy(m => m.ReleaseDate),
            ("releasedate", "desc") => q.OrderByDescending(m => m.ReleaseDate),
            _ => q.OrderBy(m => m.Title)
        };

        var total = await q.CountAsync();
        var entities = await q.Skip(page * size).Take(size).ToListAsync();
        return (entities.Select(MapToDto).ToList(), total);
    }

    public async Task<MovieDto> CreateAsync(MovieDto dto)
    {
        var movie = new Movie
        {
            Title = dto.Title!,
            Description = dto.Description,
            DurationMinutes = dto.DurationMinutes ?? 0,
            Genre = dto.Genre,
            Director = dto.Director,
            Cast = dto.Cast,
            ReleaseDate = dto.ReleaseDate,
            PosterImageUrl = dto.PosterImageUrl,
            TrailerUrl = dto.TrailerUrl,
            Rating = dto.Rating
        };
        db.Movies.Add(movie);
        await db.SaveChangesAsync();
        return MapToDto(movie);
    }

    public async Task<MovieDto> UpdateAsync(long id, MovieDto dto)
    {
        var movie = await db.Movies.FindAsync(id) ?? throw new KeyNotFoundException("Movie not found");
        if (dto.Title != null) movie.Title = dto.Title;
        if (dto.Description != null) movie.Description = dto.Description;
        if (dto.DurationMinutes.HasValue) movie.DurationMinutes = dto.DurationMinutes.Value;
        if (dto.Genre.HasValue) movie.Genre = dto.Genre;
        if (dto.Director != null) movie.Director = dto.Director;
        if (dto.Cast != null) movie.Cast = dto.Cast;
        if (dto.ReleaseDate.HasValue) movie.ReleaseDate = dto.ReleaseDate;
        if (dto.PosterImageUrl != null) movie.PosterImageUrl = dto.PosterImageUrl;
        if (dto.TrailerUrl != null) movie.TrailerUrl = dto.TrailerUrl;
        if (dto.Rating.HasValue) movie.Rating = dto.Rating;
        await db.SaveChangesAsync();
        return MapToDto(movie);
    }

    public async Task DeleteAsync(long id)
    {
        var movie = await db.Movies.FindAsync(id) ?? throw new KeyNotFoundException("Movie not found");
        db.Movies.Remove(movie);
        await db.SaveChangesAsync();
    }

    public async Task<object> GetMovieScreeningsAdminAsync(long movieId)
    {
        var movie = await db.Movies.FindAsync(movieId) ?? throw new KeyNotFoundException("Movie not found");
        var screenings = await db.Screenings
            .Include(s => s.Theatre)
            .Where(s => s.MovieId == movieId)
            .ToListAsync();
        return new { movie = MapToDto(movie), screenings = screenings.Select(s => MapScreeningToDto(s)) };
    }

    public static MovieDto MapToDto(Movie m) => new()
    {
        Id = m.Id, Title = m.Title, Description = m.Description,
        DurationMinutes = m.DurationMinutes, Genre = m.Genre, Director = m.Director,
        Cast = m.Cast, ReleaseDate = m.ReleaseDate, PosterImageUrl = m.PosterImageUrl,
        TrailerUrl = m.TrailerUrl, Rating = m.Rating
    };

    private static ScreeningDto MapScreeningToDto(Screening s) => new()
    {
        Id = s.Id, MovieId = s.MovieId, MovieTitle = s.Movie?.Title,
        TheatreId = s.TheatreId, TheatreName = s.Theatre?.Name,
        StartTime = s.StartTime, EndTime = s.EndTime,
        ScreenNumber = s.ScreenNumber, Format = s.Format, BasePrice = s.BasePrice
    };
}
