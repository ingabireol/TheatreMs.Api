using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheatreMs.Api.Common;
using TheatreMs.Api.DTOs.Movie;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Controllers;

[ApiController]
[Route("api/movies")]
public class MoviesController(IMovieService movieService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetAll([FromQuery] string? query, [FromQuery] string? genre, [FromQuery] DateOnly? date)
    {
        var movies = await movieService.GetAllAsync(query, genre, date);
        return Ok(ApiResponse<object>.Ok(movies));
    }

    [HttpGet("screenings")]
    public async Task<ActionResult<ApiResponse<object>>> GetWithScreenings([FromQuery] DateOnly? date)
    {
        var result = await movieService.GetMoviesWithScreeningsAsync(date);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<MovieDto>>> GetById(long id)
    {
        var movie = await movieService.GetByIdAsync(id);
        if (movie == null) return NotFound(ApiResponse<MovieDto>.Fail("Movie not found"));
        return Ok(ApiResponse<MovieDto>.Ok(movie));
    }

    [HttpGet("{id:long}/screenings")]
    public async Task<ActionResult<ApiResponse<object>>> GetScreenings(long id, [FromQuery] int days = 7)
    {
        var result = await movieService.GetScreeningsForMovieAsync(id, days);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<object>>> Search([FromQuery] string query)
    {
        var results = await movieService.SearchAsync(query);
        return Ok(ApiResponse<object>.Ok(results));
    }

    [HttpGet("genre/{genre}")]
    public async Task<ActionResult<ApiResponse<object>>> GetByGenre(string genre)
    {
        var results = await movieService.GetByGenreAsync(genre);
        return Ok(ApiResponse<object>.Ok(results));
    }

    [HttpGet("genres")]
    public async Task<ActionResult<ApiResponse<List<string>>>> GetGenres()
    {
        var genres = await movieService.GetGenresAsync();
        return Ok(ApiResponse<List<string>>.Ok(genres));
    }

    [HttpGet("ratings")]
    public async Task<ActionResult<ApiResponse<List<string>>>> GetRatings()
    {
        var ratings = await movieService.GetRatingsAsync();
        return Ok(ApiResponse<List<string>>.Ok(ratings));
    }

    [HttpPost]
    [Authorize(Roles = "ROLE_ADMIN,ROLE_MANAGER")]
    public async Task<ActionResult<ApiResponse<MovieDto>>> Create([FromBody] MovieDto dto)
    {
        var movie = await movieService.CreateAsync(dto);
        return Ok(ApiResponse<MovieDto>.Ok(movie, "Movie created"));
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "ROLE_ADMIN,ROLE_MANAGER")]
    public async Task<ActionResult<ApiResponse<MovieDto>>> Update(long id, [FromBody] MovieDto dto)
    {
        try
        {
            var movie = await movieService.UpdateAsync(id, dto);
            return Ok(ApiResponse<MovieDto>.Ok(movie, "Movie updated"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<MovieDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = "ROLE_ADMIN")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(long id)
    {
        try
        {
            await movieService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok("Movie deleted"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
