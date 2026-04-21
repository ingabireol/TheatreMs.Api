using Microsoft.AspNetCore.Mvc;
using TheatreMs.Api.Common;
using TheatreMs.Api.DTOs.Screening;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Controllers;

[ApiController]
[Route("api/screenings")]
public class ScreeningsController(IScreeningService screeningService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetAll([FromQuery] long? movieId, [FromQuery] long? theatreId, [FromQuery] DateOnly? date)
    {
        var results = await screeningService.GetAllAsync(movieId, theatreId, date);
        return Ok(ApiResponse<object>.Ok(results));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<ScreeningDto>>> GetById(long id)
    {
        var screening = await screeningService.GetByIdAsync(id);
        if (screening == null) return NotFound(ApiResponse<ScreeningDto>.Fail("Screening not found"));
        return Ok(ApiResponse<ScreeningDto>.Ok(screening));
    }

    [HttpGet("movie/{movieId:long}")]
    public async Task<ActionResult<ApiResponse<object>>> GetByMovie(long movieId, [FromQuery] int days = 7)
    {
        var results = await screeningService.GetByMovieAsync(movieId, days);
        return Ok(ApiResponse<object>.Ok(results));
    }

    [HttpGet("theatre/{theatreId:long}")]
    public async Task<ActionResult<ApiResponse<object>>> GetByTheatre(long theatreId, [FromQuery] DateOnly? date)
    {
        var results = await screeningService.GetByTheatreAsync(theatreId, date);
        return Ok(ApiResponse<object>.Ok(results));
    }

    [HttpGet("upcoming")]
    public async Task<ActionResult<ApiResponse<object>>> GetUpcoming([FromQuery] int days = 7)
    {
        var results = await screeningService.GetUpcomingAsync(days);
        return Ok(ApiResponse<object>.Ok(results));
    }

    [HttpGet("date-range")]
    public async Task<ActionResult<ApiResponse<object>>> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var results = await screeningService.GetByDateRangeAsync(startDate, endDate);
        return Ok(ApiResponse<object>.Ok(results));
    }

    [HttpGet("{id:long}/seats")]
    public async Task<ActionResult<ApiResponse<HashSet<string>>>> GetAvailableSeats(long id)
    {
        var seats = await screeningService.GetAvailableSeatsAsync(id);
        return Ok(ApiResponse<HashSet<string>>.Ok(seats));
    }

    [HttpGet("{id:long}/booked-seats")]
    public async Task<ActionResult<ApiResponse<HashSet<string>>>> GetBookedSeats(long id)
    {
        var seats = await screeningService.GetBookedSeatsAsync(id);
        return Ok(ApiResponse<HashSet<string>>.Ok(seats));
    }

    [HttpGet("{id:long}/layout")]
    public async Task<ActionResult<ApiResponse<object>>> GetLayout(long id)
    {
        var layout = await screeningService.GetSeatLayoutAsync(id);
        return Ok(ApiResponse<object>.Ok(layout));
    }
}
