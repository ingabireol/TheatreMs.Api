using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheatreMs.Api.Common;
using TheatreMs.Api.DTOs.Theatre;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/theatres")]
[Authorize(Roles = "ROLE_ADMIN,ROLE_MANAGER")]
public class AdminTheatresController(ITheatreService theatreService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetAll([FromQuery] string? search, [FromQuery] string sortBy = "id")
    {
        var results = await theatreService.GetAllAsync(search, sortBy);
        return Ok(ApiResponse<object>.Ok(results));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TheatreDto>>> Create([FromBody] TheatreDto dto)
    {
        var theatre = await theatreService.CreateAsync(dto);
        return Ok(ApiResponse<TheatreDto>.Ok(theatre, "Theatre created"));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<TheatreDto>>> GetById(long id)
    {
        var theatre = await theatreService.GetByIdAsync(id);
        if (theatre == null) return NotFound(ApiResponse<TheatreDto>.Fail("Theatre not found"));
        return Ok(ApiResponse<TheatreDto>.Ok(theatre));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<TheatreDto>>> Update(long id, [FromBody] TheatreDto dto)
    {
        try
        {
            var theatre = await theatreService.UpdateAsync(id, dto);
            return Ok(ApiResponse<TheatreDto>.Ok(theatre, "Theatre updated"));
        }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<TheatreDto>.Fail(ex.Message)); }
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = "ROLE_ADMIN")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(long id)
    {
        try
        {
            await theatreService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok("Theatre deleted"));
        }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<object>.Fail(ex.Message)); }
    }

    [HttpGet("{id:long}/seats")]
    public async Task<ActionResult<ApiResponse<object>>> GetSeats(long id)
    {
        var seats = await theatreService.GetTheatreSeatsAsync(id);
        return Ok(ApiResponse<object>.Ok(seats));
    }

    [HttpPost("{id:long}/seats/initialize")]
    public async Task<ActionResult<ApiResponse<object>>> InitializeSeats(
        long id, [FromQuery] int screenNumber, [FromQuery] int rows, [FromQuery] int seatsPerRow)
    {
        await theatreService.InitializeSeatsAsync(id, screenNumber, rows, seatsPerRow);
        return Ok(ApiResponse<object>.Ok("Seats initialized"));
    }

    [HttpGet("{id:long}/screenings")]
    public async Task<ActionResult<ApiResponse<object>>> GetScreenings(long id)
    {
        try
        {
            var result = await theatreService.GetTheatreScreeningsAsync(id);
            return Ok(ApiResponse<object>.Ok(result));
        }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<object>.Fail(ex.Message)); }
    }
}
