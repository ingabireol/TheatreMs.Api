namespace TheatreMs.Api.Services.Interfaces;

public interface ISearchService
{
    Task<Dictionary<string, object>> GlobalSearchAsync(string query, int limit);
    Task<Dictionary<string, object>> SearchMoviesAsync(string query, int limit);
    Task<Dictionary<string, object>> SearchTheatresAsync(string query, int limit);
    Task<Dictionary<string, object>> SearchScreeningsAsync(string query, int limit);
    Task<Dictionary<string, object>> SearchUsersAsync(string query, int limit);
    Task<Dictionary<string, object>> GetSuggestionsAsync(string query, int limit);
}
