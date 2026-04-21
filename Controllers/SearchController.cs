using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheatreMs.Api.Common;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController(ISearchService searchService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<Dictionary<string, object>>>> Global([FromQuery] string query, [FromQuery] int limit = 3)
    {
        var result = await searchService.GlobalSearchAsync(query, limit);
        return Ok(ApiResponse<Dictionary<string, object>>.Ok(result));
    }

    [HttpGet("movies")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, object>>>> Movies([FromQuery] string query, [FromQuery] int limit = 10)
    {
        var result = await searchService.SearchMoviesAsync(query, limit);
        return Ok(ApiResponse<Dictionary<string, object>>.Ok(result));
    }

    [HttpGet("theatres")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, object>>>> Theatres([FromQuery] string query, [FromQuery] int limit = 10)
    {
        var result = await searchService.SearchTheatresAsync(query, limit);
        return Ok(ApiResponse<Dictionary<string, object>>.Ok(result));
    }

    [HttpGet("screenings")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, object>>>> Screenings([FromQuery] string query, [FromQuery] int limit = 10)
    {
        var result = await searchService.SearchScreeningsAsync(query, limit);
        return Ok(ApiResponse<Dictionary<string, object>>.Ok(result));
    }

    [HttpGet("users")]
    [Authorize(Roles = "ROLE_ADMIN")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, object>>>> Users([FromQuery] string query, [FromQuery] int limit = 10)
    {
        var result = await searchService.SearchUsersAsync(query, limit);
        return Ok(ApiResponse<Dictionary<string, object>>.Ok(result));
    }

    [HttpGet("suggestions")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, object>>>> Suggestions([FromQuery] string query, [FromQuery] int limit = 5)
    {
        var result = await searchService.GetSuggestionsAsync(query, limit);
        return Ok(ApiResponse<Dictionary<string, object>>.Ok(result));
    }
}
