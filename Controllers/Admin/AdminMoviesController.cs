using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheatreMs.Api.Common;
using TheatreMs.Api.DTOs.Movie;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/movies")]
[Authorize(Roles = "ROLE_ADMIN,ROLE_MANAGER")]
public class AdminMoviesController(IMovieService movieService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetPaged(
        [FromQuery] string? search, [FromQuery] string? genre,
        [FromQuery] string sortBy = "title", [FromQuery] string sortOrder = "asc",
        [FromQuery] int page = 0, [FromQuery] int size = 10)
    {
        var (items, total) = await movieService.GetPagedAsync(search, genre, sortBy, sortOrder, page, size);
        var totalPages = size > 0 ? (int)Math.Ceiling((double)total / size) : 0;
        return Ok(ApiResponse<object>.Ok(new
        {
            movies = items,
            totalElements = total,
            totalPages,
            hasNext = (page + 1) * size < total,
            hasPrevious = page > 0,
            currentPage = page,
            pageSize = size
        }));
    }

    [HttpGet("genres")]
    public async Task<ActionResult<ApiResponse<List<string>>>> GetGenres() =>
        Ok(ApiResponse<List<string>>.Ok(await movieService.GetGenresAsync()));

    [HttpGet("ratings")]
    public async Task<ActionResult<ApiResponse<List<string>>>> GetRatings() =>
        Ok(ApiResponse<List<string>>.Ok(await movieService.GetRatingsAsync()));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MovieDto>>> Create([FromBody] MovieDto dto)
    {
        var movie = await movieService.CreateAsync(dto);
        return Ok(ApiResponse<MovieDto>.Ok(movie, "Movie created"));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<MovieDto>>> GetById(long id)
    {
        var movie = await movieService.GetByIdAsync(id);
        if (movie == null) return NotFound(ApiResponse<MovieDto>.Fail("Movie not found"));
        return Ok(ApiResponse<MovieDto>.Ok(movie));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<MovieDto>>> Update(long id, [FromBody] MovieDto dto)
    {
        try
        {
            var movie = await movieService.UpdateAsync(id, dto);
            return Ok(ApiResponse<MovieDto>.Ok(movie, "Movie updated"));
        }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<MovieDto>.Fail(ex.Message)); }
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
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<object>.Fail(ex.Message)); }
    }

    [HttpGet("{id:long}/screenings")]
    public async Task<ActionResult<ApiResponse<object>>> GetScreenings(long id)
    {
        try
        {
            var result = await movieService.GetMovieScreeningsAdminAsync(id);
            return Ok(ApiResponse<object>.Ok(result));
        }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<object>.Fail(ex.Message)); }
    }
}
