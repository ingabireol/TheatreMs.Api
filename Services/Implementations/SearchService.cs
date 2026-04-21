using Microsoft.EntityFrameworkCore;
using TheatreMs.Api.Data;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Services.Implementations;

public class SearchService(AppDbContext db) : ISearchService
{
    public async Task<Dictionary<string, object>> GlobalSearchAsync(string query, int limit)
    {
        var movies = await db.Movies.Where(m => m.Title.Contains(query)).Take(limit).Select(m => new { m.Id, m.Title, m.Genre, m.PosterImageUrl }).ToListAsync();
        var theatres = await db.Theatres.Where(t => t.Name.Contains(query) || t.Address.Contains(query)).Take(limit).Select(t => new { t.Id, t.Name, t.Address }).ToListAsync();
        var screenings = await db.Screenings.Include(s => s.Movie).Where(s => s.Movie.Title.Contains(query)).Take(limit).Select(s => new { s.Id, movieTitle = s.Movie.Title, s.StartTime, s.Format }).ToListAsync();
        return new() { ["movies"] = movies, ["theatres"] = theatres, ["screenings"] = screenings };
    }

    public async Task<Dictionary<string, object>> SearchMoviesAsync(string query, int limit)
    {
        var results = await db.Movies
            .Where(m => m.Title.Contains(query) || (m.Director != null && m.Director.Contains(query)) || (m.Cast != null && m.Cast.Contains(query)))
            .Take(limit)
            .Select(m => new { m.Id, m.Title, m.Genre, m.Director, m.PosterImageUrl, m.Rating })
            .ToListAsync();
        return new() { ["movies"] = results, ["count"] = results.Count };
    }

    public async Task<Dictionary<string, object>> SearchTheatresAsync(string query, int limit)
    {
        var results = await db.Theatres
            .Where(t => t.Name.Contains(query) || t.Address.Contains(query))
            .Take(limit)
            .Select(t => new { t.Id, t.Name, t.Address, t.PhoneNumber })
            .ToListAsync();
        return new() { ["theatres"] = results, ["count"] = results.Count };
    }

    public async Task<Dictionary<string, object>> SearchScreeningsAsync(string query, int limit)
    {
        var results = await db.Screenings.Include(s => s.Movie).Include(s => s.Theatre)
            .Where(s => s.Movie.Title.Contains(query) || s.Theatre.Name.Contains(query))
            .Take(limit)
            .Select(s => new { s.Id, movieTitle = s.Movie.Title, theatreName = s.Theatre.Name, s.StartTime, s.Format })
            .ToListAsync();
        return new() { ["screenings"] = results, ["count"] = results.Count };
    }

    public async Task<Dictionary<string, object>> SearchUsersAsync(string query, int limit)
    {
        var results = await db.Users
            .Where(u => u.Username.Contains(query) || u.Email.Contains(query) || u.FirstName.Contains(query) || u.LastName.Contains(query))
            .Take(limit)
            .Select(u => new { u.Id, u.Username, u.Email, u.FirstName, u.LastName, role = u.Role.ToString() })
            .ToListAsync();
        return new() { ["users"] = results, ["count"] = results.Count };
    }

    public async Task<Dictionary<string, object>> GetSuggestionsAsync(string query, int limit)
    {
        var movies = await db.Movies.Where(m => m.Title.Contains(query)).Take(limit).Select(m => new { type = "movie", m.Id, label = m.Title }).ToListAsync<object>();
        var theatres = await db.Theatres.Where(t => t.Name.Contains(query)).Take(limit).Select(t => new { type = "theatre", t.Id, label = t.Name }).ToListAsync<object>();
        var suggestions = movies.Concat(theatres).Take(limit).ToList();
        return new() { ["suggestions"] = suggestions };
    }
}
