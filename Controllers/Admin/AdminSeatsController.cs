using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheatreMs.Api.Common;
using TheatreMs.Api.DTOs.Seat;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/seats")]
[Authorize(Roles = "ROLE_ADMIN")]
public class AdminSeatsController(ISeatService seatService) : ControllerBase
{
    [HttpGet("theatre/{theatreId:long}/screens")]
    public async Task<ActionResult<ApiResponse<object>>> GetScreens(long theatreId)
    {
        var result = await seatService.GetTheatreScreensAsync(theatreId);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("theatre/{theatreId:long}/screen/{screenNumber:int}")]
    public async Task<ActionResult<List<SeatDto>>> GetScreenSeats(long theatreId, int screenNumber)
    {
        var seats = await seatService.GetScreenSeatsAsync(theatreId, screenNumber);
        return Ok(seats);
    }

    [HttpPost("theatre/{theatreId:long}/screen/{screenNumber:int}/initialize")]
    public async Task<ActionResult<ApiResponse<string>>> Initialize(
        long theatreId, int screenNumber, [FromQuery] int rows, [FromQuery] int seatsPerRow)
    {
        var result = await seatService.InitializeScreenAsync(theatreId, screenNumber, rows, seatsPerRow);
        return Ok(ApiResponse<string>.Ok(result));
    }

    [HttpPut("{seatId:long}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSeat(
        long seatId, [FromQuery] string seatType, [FromQuery] double priceMultiplier)
    {
        try
        {
            await seatService.UpdateSeatAsync(seatId, seatType, priceMultiplier);
            return Ok(ApiResponse<object>.Ok("Seat updated"));
        }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<object>.Fail(ex.Message)); }
    }

    [HttpPut("updateRow")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateRow(
        [FromQuery] long theatreId, [FromQuery] int screenNumber,
        [FromQuery] string rowName, [FromQuery] string seatType, [FromQuery] double priceMultiplier)
    {
        await seatService.UpdateRowAsync(theatreId, screenNumber, rowName, seatType, priceMultiplier);
        return Ok(ApiResponse<object>.Ok("Row updated"));
    }

    [HttpPut("bulkUpdate")]
    public async Task<ActionResult<ApiResponse<string>>> BulkUpdate(
        [FromQuery] long theatreId, [FromQuery] int screenNumber,
        [FromQuery] string selection, [FromQuery] string seatType, [FromQuery] double priceMultiplier)
    {
        var result = await seatService.BulkUpdateAsync(theatreId, screenNumber, selection, seatType, priceMultiplier);
        return Ok(ApiResponse<string>.Ok(result));
    }

    [HttpDelete("theatre/{theatreId:long}/screen/{screenNumber:int}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteScreen(long theatreId, int screenNumber)
    {
        var result = await seatService.DeleteScreenAsync(theatreId, screenNumber);
        return Ok(ApiResponse<string>.Ok(result));
    }
}
