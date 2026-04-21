using Microsoft.EntityFrameworkCore;
using TheatreMs.Api.Data;
using TheatreMs.Api.DTOs.Screening;
using TheatreMs.Api.Models;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Services.Implementations;

public class ScreeningService(AppDbContext db) : IScreeningService
{
    public async Task<List<ScreeningDto>> GetAllAsync(long? movieId, long? theatreId, DateOnly? date)
    {
        var q = db.Screenings.Include(s => s.Movie).Include(s => s.Theatre).AsQueryable();
        if (movieId.HasValue) q = q.Where(s => s.MovieId == movieId);
        if (theatreId.HasValue) q = q.Where(s => s.TheatreId == theatreId);
        if (date.HasValue) q = q.Where(s => DateOnly.FromDateTime(s.StartTime) == date.Value);
        var entities = await q.ToListAsync();
        return entities.Select(MapToDto).ToList();
    }

    public async Task<ScreeningDto?> GetByIdAsync(long id)
    {
        var s = await db.Screenings.Include(s => s.Movie).Include(s => s.Theatre).FirstOrDefaultAsync(s => s.Id == id);
        return s == null ? null : MapToDto(s);
    }

    public async Task<List<ScreeningDto>> GetByMovieAsync(long movieId, int days)
    {
        var until = DateTime.UtcNow.AddDays(days);
        var entities = await db.Screenings.Include(s => s.Movie).Include(s => s.Theatre)
            .Where(s => s.MovieId == movieId && s.StartTime >= DateTime.UtcNow && s.StartTime <= until)
            .ToListAsync();
        return entities.Select(MapToDto).ToList();
    }

    public async Task<List<ScreeningDto>> GetByTheatreAsync(long theatreId, DateOnly? date)
    {
        var q = db.Screenings.Include(s => s.Movie).Include(s => s.Theatre)
            .Where(s => s.TheatreId == theatreId);
        if (date.HasValue) q = q.Where(s => DateOnly.FromDateTime(s.StartTime) == date.Value);
        var entities = await q.ToListAsync();
        return entities.Select(MapToDto).ToList();
    }

    public async Task<Dictionary<string, List<ScreeningDto>>> GetUpcomingAsync(int days)
    {
        var until = DateTime.UtcNow.AddDays(days);
        var screenings = await db.Screenings.Include(s => s.Movie).Include(s => s.Theatre)
            .Where(s => s.StartTime >= DateTime.UtcNow && s.StartTime <= until)
            .ToListAsync();
        return screenings
            .GroupBy(s => s.Movie?.Title ?? "Unknown")
            .ToDictionary(g => g.Key, g => g.Select(s => MapToDto(s)).ToList());
    }

    public async Task<List<ScreeningDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var entities = await db.Screenings.Include(s => s.Movie).Include(s => s.Theatre)
            .Where(s => s.StartTime >= startDate && s.StartTime <= endDate)
            .ToListAsync();
        return entities.Select(MapToDto).ToList();
    }

    public async Task<HashSet<string>> GetAvailableSeatsAsync(long screeningId)
    {
        var screening = await db.Screenings.FindAsync(screeningId) ?? throw new KeyNotFoundException("Screening not found");
        var allSeats = await db.Seats
            .Where(s => s.TheatreId == screening.TheatreId && s.ScreenNumber == screening.ScreenNumber)
            .Select(s => s.RowName + s.SeatNumber).ToListAsync();
        var booked = await GetBookedSeatsAsync(screeningId);
        return allSeats.Where(s => !booked.Contains(s)).ToHashSet();
    }

    public async Task<HashSet<string>> GetBookedSeatsAsync(long screeningId)
    {
        var bookings = await db.Bookings
            .Where(b => b.ScreeningId == screeningId && b.PaymentStatus != PaymentStatus.CANCELLED && b.PaymentStatus != PaymentStatus.REFUNDED)
            .ToListAsync();
        return bookings.SelectMany(b => b.BookedSeats).ToHashSet();
    }

    public async Task<object> GetSeatLayoutAsync(long screeningId)
    {
        var screening = await db.Screenings.FindAsync(screeningId) ?? throw new KeyNotFoundException("Screening not found");
        var seats = await db.Seats
            .Where(s => s.TheatreId == screening.TheatreId && s.ScreenNumber == screening.ScreenNumber)
            .ToListAsync();
        var booked = await GetBookedSeatsAsync(screeningId);
        return new
        {
            screeningId,
            seats = seats.Select(s => new
            {
                label = s.RowName + s.SeatNumber, row = s.RowName, number = s.SeatNumber,
                seatType = s.SeatType.ToString(), priceMultiplier = s.PriceMultiplier,
                isBooked = booked.Contains(s.RowName + s.SeatNumber)
            })
        };
    }

    public async Task<(List<ScreeningDto> Items, int TotalCount)> GetPagedAsync(long? movieId, long? theatreId, DateOnly? date, string? search, string sortBy, string sortOrder, int page, int size)
    {
        var q = db.Screenings.Include(s => s.Movie).Include(s => s.Theatre).AsQueryable();
        if (movieId.HasValue) q = q.Where(s => s.MovieId == movieId);
        if (theatreId.HasValue) q = q.Where(s => s.TheatreId == theatreId);
        if (date.HasValue) q = q.Where(s => DateOnly.FromDateTime(s.StartTime) == date.Value);
        if (!string.IsNullOrEmpty(search))
            q = q.Where(s => s.Movie!.Title.Contains(search) || s.Theatre!.Name.Contains(search));

        q = (sortBy.ToLower(), sortOrder.ToLower()) switch
        {
            ("starttime", "desc") => q.OrderByDescending(s => s.StartTime),
            ("movie", _) => q.OrderBy(s => s.Movie!.Title),
            _ => q.OrderBy(s => s.StartTime)
        };

        var total = await q.CountAsync();
        var entities = await q.Skip(page * size).Take(size).ToListAsync();
        return (entities.Select(MapToDto).ToList(), total);
    }

    public Task<List<string>> GetFormatsAsync() =>
        Task.FromResult(Enum.GetNames<ScreeningFormat>().ToList());

    public async Task<object> GetScreeningAdminDetailAsync(long id)
    {
        var s = await db.Screenings.Include(s => s.Movie).Include(s => s.Theatre)
            .FirstOrDefaultAsync(s => s.Id == id) ?? throw new KeyNotFoundException("Screening not found");
        var bookings = await db.Bookings.Where(b => b.ScreeningId == id).CountAsync();
        return new { screening = MapToDto(s), bookingCount = bookings };
    }

    public async Task<ScreeningDto> CreateAsync(ScreeningDto dto)
    {
        var movie = await db.Movies.FindAsync(dto.MovieId) ?? throw new KeyNotFoundException("Movie not found");
        var screening = new Screening
        {
            MovieId = dto.MovieId!.Value, TheatreId = dto.TheatreId!.Value,
            StartTime = dto.StartTime!.Value, ScreenNumber = dto.ScreenNumber!.Value,
            Format = dto.Format ?? ScreeningFormat.STANDARD, BasePrice = dto.BasePrice ?? 0,
            EndTime = dto.StartTime.Value.AddMinutes(movie.DurationMinutes)
        };
        db.Screenings.Add(screening);
        await db.SaveChangesAsync();
        await db.Entry(screening).Reference(s => s.Movie).LoadAsync();
        await db.Entry(screening).Reference(s => s.Theatre).LoadAsync();
        return MapToDto(screening);
    }

    public async Task<ScreeningDto> UpdateAsync(long id, ScreeningDto dto)
    {
        var screening = await db.Screenings.Include(s => s.Movie).Include(s => s.Theatre)
            .FirstOrDefaultAsync(s => s.Id == id) ?? throw new KeyNotFoundException("Screening not found");
        if (dto.MovieId.HasValue) screening.MovieId = dto.MovieId.Value;
        if (dto.TheatreId.HasValue) screening.TheatreId = dto.TheatreId.Value;
        if (dto.StartTime.HasValue) screening.StartTime = dto.StartTime.Value;
        if (dto.ScreenNumber.HasValue) screening.ScreenNumber = dto.ScreenNumber.Value;
        if (dto.Format.HasValue) screening.Format = dto.Format.Value;
        if (dto.BasePrice.HasValue) screening.BasePrice = dto.BasePrice.Value;
        if (dto.StartTime.HasValue)
            screening.EndTime = dto.StartTime.Value.AddMinutes(screening.Movie.DurationMinutes);
        await db.SaveChangesAsync();
        return MapToDto(screening);
    }

    public async Task DeleteAsync(long id)
    {
        var screening = await db.Screenings.FindAsync(id) ?? throw new KeyNotFoundException("Screening not found");
        db.Screenings.Remove(screening);
        await db.SaveChangesAsync();
    }

    public async Task<List<object>> GetScreeningBookingsAsync(long id)
    {
        var bookings = await db.Bookings.Include(b => b.User)
            .Where(b => b.ScreeningId == id).ToListAsync();
        return bookings.Select(b => (object)new
        {
            id = b.Id, bookingNumber = b.BookingNumber, username = b.User.Username,
            totalAmount = b.TotalAmount, paymentStatus = b.PaymentStatus.ToString(),
            bookedSeats = b.BookedSeats
        }).ToList();
    }

    public static ScreeningDto MapToDto(Screening s) => new()
    {
        Id = s.Id, MovieId = s.MovieId, MovieTitle = s.Movie?.Title,
        TheatreId = s.TheatreId, TheatreName = s.Theatre?.Name,
        StartTime = s.StartTime, EndTime = s.EndTime,
        ScreenNumber = s.ScreenNumber, Format = s.Format, BasePrice = s.BasePrice
    };
}
