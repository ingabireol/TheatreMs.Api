using TheatreMs.Api.DTOs.Booking;

namespace TheatreMs.Api.Services.Interfaces;

public interface IBookingService
{
    Task<List<BookingDto>> GetUserBookingsAsync(long userId);
    Task<BookingDto?> GetBookingByIdAsync(long id, long userId, bool isAdmin);
    Task<BookingDto> CreateBookingAsync(BookingDto dto, long userId);
    Task CancelBookingAsync(long id, long userId, bool isAdmin);
    Task<List<BookingDto>> GetAllBookingsAsync();
}
