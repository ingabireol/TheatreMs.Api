using Microsoft.EntityFrameworkCore;
using TheatreMs.Api.Data;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Services.Implementations;

public class DashboardService(AppDbContext db) : IDashboardService
{
    public async Task<Dictionary<string, object>> GetAdminDashboardAsync()
    {
        var totalMovies = await db.Movies.CountAsync();
        var totalTheatres = await db.Theatres.CountAsync();
        var totalUsers = await db.Users.CountAsync();
        var totalBookings = await db.Bookings.CountAsync();
        var revenue = await db.Bookings.Where(b => b.PaymentStatus == Models.PaymentStatus.COMPLETED).SumAsync(b => b.TotalAmount);
        var upcomingScreenings = await db.Screenings.Where(s => s.StartTime >= DateTime.UtcNow).CountAsync();

        var recentBookings = await db.Bookings
            .Include(b => b.User).Include(b => b.Screening).ThenInclude(s => s.Movie)
            .OrderByDescending(b => b.BookingTime).Take(5)
            .Select(b => new { b.Id, b.BookingNumber, b.User.Username, movieTitle = b.Screening.Movie.Title, b.TotalAmount, b.BookingTime })
            .ToListAsync();

        return new()
        {
            ["totalMovies"] = totalMovies,
            ["totalTheatres"] = totalTheatres,
            ["totalUsers"] = totalUsers,
            ["totalBookings"] = totalBookings,
            ["totalRevenue"] = revenue,
            ["upcomingScreenings"] = upcomingScreenings,
            ["recentBookings"] = recentBookings
        };
    }

    public async Task<Dictionary<string, object>> GetUserDashboardAsync(long userId)
    {
        var bookings = await db.Bookings
            .Include(b => b.Screening).ThenInclude(s => s.Movie)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingTime).Take(5)
            .Select(b => new { b.Id, b.BookingNumber, movieTitle = b.Screening.Movie.Title, b.Screening.StartTime, b.TotalAmount, status = b.PaymentStatus.ToString() })
            .ToListAsync();

        var totalSpent = await db.Bookings
            .Where(b => b.UserId == userId && b.PaymentStatus == Models.PaymentStatus.COMPLETED)
            .SumAsync(b => b.TotalAmount);

        return new()
        {
            ["recentBookings"] = bookings,
            ["totalBookings"] = await db.Bookings.CountAsync(b => b.UserId == userId),
            ["totalSpent"] = totalSpent
        };
    }
}
