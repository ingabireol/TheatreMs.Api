using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheatreMs.Api.Common;
using TheatreMs.Api.DTOs.Screening;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/screenings")]
[Authorize(Roles = "ROLE_ADMIN,ROLE_MANAGER")]
public class AdminScreeningsController(IScreeningService screeningService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetPaged(
        [FromQuery] long? movieId, [FromQuery] long? theatreId, [FromQuery] DateOnly? date,
        [FromQuery] string? search, [FromQuery] string sortBy = "startTime",
        [FromQuery] string sortOrder = "asc", [FromQuery] int page = 0, [FromQuery] int size = 10)
    {
        var (items, total) = await screeningService.GetPagedAsync(movieId, theatreId, date, search, sortBy, sortOrder, page, size);
        var totalPages = size > 0 ? (int)Math.Ceiling((double)total / size) : 0;
        return Ok(ApiResponse<object>.Ok(new
        {
            screenings = items,
            currentPage = page,
            totalPages,
            totalElements = total,
            pageSize = size,
            hasNext = (page + 1) * size < total,
            hasPrevious = page > 0,
            isFirst = page == 0,
            isLast = (page + 1) * size >= total
        }));
    }

    [HttpGet("formats")]
    public async Task<ActionResult<ApiResponse<List<string>>>> GetFormats() =>
        Ok(ApiResponse<List<string>>.Ok(await screeningService.GetFormatsAsync()));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<object>>> GetById(long id)
    {
        try
        {
            var result = await screeningService.GetScreeningAdminDetailAsync(id);
            return Ok(ApiResponse<object>.Ok(result));
        }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<object>.Fail(ex.Message)); }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ScreeningDto>>> Create([FromBody] ScreeningDto dto)
    {
        try
        {
            var screening = await screeningService.CreateAsync(dto);
            return Ok(ApiResponse<ScreeningDto>.Ok(screening, "Screening created"));
        }
        catch (Exception ex) { return BadRequest(ApiResponse<ScreeningDto>.Fail(ex.Message)); }
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<ScreeningDto>>> Update(long id, [FromBody] ScreeningDto dto)
    {
        try
        {
            var screening = await screeningService.UpdateAsync(id, dto);
            return Ok(ApiResponse<ScreeningDto>.Ok(screening, "Screening updated"));
        }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<ScreeningDto>.Fail(ex.Message)); }
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(long id)
    {
        try
        {
            await screeningService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok("Screening deleted"));
        }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<object>.Fail(ex.Message)); }
    }

    [HttpGet("{id:long}/bookings")]
    public async Task<ActionResult<ApiResponse<object>>> GetBookings(long id)
    {
        var bookings = await screeningService.GetScreeningBookingsAsync(id);
        return Ok(ApiResponse<object>.Ok(bookings));
    }
}
