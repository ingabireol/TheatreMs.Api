using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheatreMs.Api.Common;
using TheatreMs.Api.DTOs.Booking;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize(Roles = "ROLE_USER,ROLE_MANAGER,ROLE_ADMIN")]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    private long CurrentUserId => long.Parse(User.FindFirstValue("userId")!);
    private bool IsAdmin => User.IsInRole("ROLE_ADMIN") || User.IsInRole("ROLE_MANAGER");

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetMyBookings()
    {
        var bookings = await bookingService.GetUserBookingsAsync(CurrentUserId);
        return Ok(ApiResponse<object>.Ok(bookings));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<BookingDto>>> GetById(long id)
    {
        var booking = await bookingService.GetBookingByIdAsync(id, CurrentUserId, IsAdmin);
        if (booking == null) return NotFound(ApiResponse<BookingDto>.Fail("Booking not found"));
        return Ok(ApiResponse<BookingDto>.Ok(booking));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<BookingDto>>> Create([FromBody] BookingDto dto)
    {
        try
        {
            var booking = await bookingService.CreateBookingAsync(dto, CurrentUserId);
            return Ok(ApiResponse<BookingDto>.Ok(booking, "Booking created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<BookingDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiResponse<object>>> Cancel(long id)
    {
        try
        {
            await bookingService.CancelBookingAsync(id, CurrentUserId, IsAdmin);
            return Ok(ApiResponse<object>.Ok("Booking cancelled"));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpGet("admin/all")]
    [Authorize(Roles = "ROLE_ADMIN,ROLE_MANAGER")]
    public async Task<ActionResult<ApiResponse<object>>> GetAll()
    {
        var bookings = await bookingService.GetAllBookingsAsync();
        return Ok(ApiResponse<object>.Ok(bookings));
    }
}
