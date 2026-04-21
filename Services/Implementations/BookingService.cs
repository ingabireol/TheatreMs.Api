using Microsoft.EntityFrameworkCore;
using TheatreMs.Api.Data;
using TheatreMs.Api.DTOs.Booking;
using TheatreMs.Api.Models;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Services.Implementations;

public class BookingService(AppDbContext db, IEmailService emailService) : IBookingService
{
    public async Task<List<BookingDto>> GetUserBookingsAsync(long userId)
    {
        var entities = await db.Bookings
            .Include(b => b.Screening).ThenInclude(s => s.Movie)
            .Include(b => b.Screening).ThenInclude(s => s.Theatre)
            .Include(b => b.User)
            .Where(b => b.UserId == userId)
            .ToListAsync();
        return entities.Select(MapToDto).ToList();
    }

    public async Task<BookingDto?> GetBookingByIdAsync(long id, long userId, bool isAdmin)
    {
        var booking = await db.Bookings
            .Include(b => b.Screening).ThenInclude(s => s.Movie)
            .Include(b => b.Screening).ThenInclude(s => s.Theatre)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null) return null;
        if (!isAdmin && booking.UserId != userId) return null;
        return MapToDto(booking);
    }

    public async Task<BookingDto> CreateBookingAsync(BookingDto dto, long userId)
    {
        if (dto.ScreeningId == null || dto.BookedSeats == null || !dto.BookedSeats.Any())
            throw new InvalidOperationException("Screening and seats are required");

        var screening = await db.Screenings.Include(s => s.Movie).Include(s => s.Theatre)
            .FirstOrDefaultAsync(s => s.Id == dto.ScreeningId)
            ?? throw new KeyNotFoundException("Screening not found");

        // Check seat availability — load bookings first, then flatten in memory
        var existingBookings = await db.Bookings
            .Where(b => b.ScreeningId == dto.ScreeningId
                && b.PaymentStatus != PaymentStatus.CANCELLED
                && b.PaymentStatus != PaymentStatus.REFUNDED)
            .ToListAsync();
        var alreadyBooked = existingBookings.SelectMany(b => b.BookedSeats).ToHashSet();

        var conflicts = dto.BookedSeats.Intersect(alreadyBooked).ToList();
        if (conflicts.Any())
            throw new InvalidOperationException($"Seats already booked: {string.Join(", ", conflicts)}");

        // Calculate price
        double totalAmount = 0;
        foreach (var seatLabel in dto.BookedSeats)
        {
            var rowName = new string(seatLabel.TakeWhile(char.IsLetter).ToArray());
            var seat = await db.Seats.FirstOrDefaultAsync(s =>
                s.TheatreId == screening.TheatreId && s.ScreenNumber == screening.ScreenNumber && s.RowName == rowName);
            totalAmount += screening.BasePrice * (seat?.PriceMultiplier ?? 1.0);
        }

        var booking = new Booking
        {
            UserId = userId,
            ScreeningId = dto.ScreeningId.Value,
            BookingNumber = "BK" + DateTime.UtcNow.Ticks.ToString()[^8..],
            BookingTime = DateTime.UtcNow,
            TotalAmount = totalAmount,
            PaymentStatus = PaymentStatus.COMPLETED,
            PaymentMethod = dto.PaymentMethod ?? "CARD",
            BookedSeats = dto.BookedSeats
        };

        db.Bookings.Add(booking);
        await db.SaveChangesAsync();

        var user = await db.Users.FindAsync(userId);
        if (user != null)
            await emailService.SendBookingConfirmationAsync(user.Email, booking.BookingNumber,
                screening.Movie.Title, screening.StartTime);

        await db.Entry(booking).Reference(b => b.User).LoadAsync();
        await db.Entry(booking).Reference(b => b.Screening).LoadAsync();
        return MapToDto(booking);
    }

    public async Task CancelBookingAsync(long id, long userId, bool isAdmin)
    {
        var booking = await db.Bookings.FindAsync(id) ?? throw new KeyNotFoundException("Booking not found");
        if (!isAdmin && booking.UserId != userId)
            throw new UnauthorizedAccessException("Cannot cancel another user's booking");
        booking.PaymentStatus = PaymentStatus.CANCELLED;
        await db.SaveChangesAsync();
    }

    public async Task<List<BookingDto>> GetAllBookingsAsync()
    {
        var entities = await db.Bookings
            .Include(b => b.Screening).ThenInclude(s => s.Movie)
            .Include(b => b.Screening).ThenInclude(s => s.Theatre)
            .Include(b => b.User)
            .ToListAsync();
        return entities.Select(MapToDto).ToList();
    }

    public static BookingDto MapToDto(Booking b) => new()
    {
        Id = b.Id, BookingNumber = b.BookingNumber,
        UserId = b.UserId, Username = b.User?.Username, UserEmail = b.User?.Email,
        ScreeningId = b.ScreeningId,
        MovieTitle = b.Screening?.Movie?.Title, MovieId = b.Screening?.MovieId,
        TheatreId = b.Screening?.TheatreId, TheatreName = b.Screening?.Theatre?.Name,
        MovieUrl = b.Screening?.Movie?.PosterImageUrl,
        ScreeningTime = b.Screening?.StartTime, BookingTime = b.BookingTime,
        TotalAmount = b.TotalAmount, PaymentStatus = b.PaymentStatus,
        BookedSeats = b.BookedSeats, PaymentMethod = b.PaymentMethod
    };
}
