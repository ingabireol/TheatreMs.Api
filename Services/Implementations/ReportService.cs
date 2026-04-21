using Microsoft.EntityFrameworkCore;
using TheatreMs.Api.Data;
using TheatreMs.Api.Models;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Services.Implementations;

public class ReportService(AppDbContext db) : IReportService
{
    public async Task<object> GetBookingReportAsync(DateTime from, DateTime to)
    {
        var bookings = await db.Bookings
            .Include(b => b.Screening).ThenInclude(s => s.Movie)
            .Where(b => b.BookingTime >= from && b.BookingTime <= to)
            .ToListAsync();

        var rows = bookings
            .GroupBy(b => DateOnly.FromDateTime(b.BookingTime))
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                date = g.Key.ToString("yyyy-MM-dd"),
                bookingCount = g.Count(),
                revenue = Math.Round(g.Where(b => b.PaymentStatus == PaymentStatus.COMPLETED).Sum(b => b.TotalAmount), 2),
                completed = g.Count(b => b.PaymentStatus == PaymentStatus.COMPLETED),
                cancelled = g.Count(b => b.PaymentStatus == PaymentStatus.CANCELLED),
                seatsSold = g.Where(b => b.PaymentStatus == PaymentStatus.COMPLETED).Sum(b => b.BookedSeats.Count)
            })
            .ToList();

        return new
        {
            rows,
            summary = new
            {
                totalBookings = bookings.Count,
                totalRevenue = Math.Round(bookings.Where(b => b.PaymentStatus == PaymentStatus.COMPLETED).Sum(b => b.TotalAmount), 2),
                completedCount = bookings.Count(b => b.PaymentStatus == PaymentStatus.COMPLETED),
                cancelledCount = bookings.Count(b => b.PaymentStatus == PaymentStatus.CANCELLED),
                totalSeatsSold = bookings.Where(b => b.PaymentStatus == PaymentStatus.COMPLETED).Sum(b => b.BookedSeats.Count)
            }
        };
    }

    public async Task<object> GetRevenueReportAsync(DateTime from, DateTime to)
    {
        var bookings = await db.Bookings
            .Include(b => b.Screening).ThenInclude(s => s.Movie)
            .Where(b => b.BookingTime >= from && b.BookingTime <= to && b.PaymentStatus == PaymentStatus.COMPLETED)
            .ToListAsync();

        var rows = bookings
            .GroupBy(b => b.Screening?.Movie?.Title ?? "Unknown")
            .Select(g => new
            {
                movieTitle = g.Key,
                bookingCount = g.Count(),
                revenue = Math.Round(g.Sum(b => b.TotalAmount), 2),
                avgBookingValue = Math.Round(g.Average(b => b.TotalAmount), 2),
                seatsSold = g.Sum(b => b.BookedSeats.Count)
            })
            .OrderByDescending(r => r.revenue)
            .ToList();

        return new
        {
            rows,
            summary = new
            {
                totalRevenue = Math.Round(rows.Sum(r => r.revenue), 2),
                totalBookings = rows.Sum(r => r.bookingCount),
                totalSeatsSold = rows.Sum(r => r.seatsSold),
                uniqueMovies = rows.Count
            }
        };
    }

    public async Task<object> GetMoviePerformanceReportAsync(DateTime from, DateTime to)
    {
        var movies = await db.Movies.ToListAsync();

        var screenings = await db.Screenings
            .Where(s => s.StartTime >= from && s.StartTime <= to)
            .ToListAsync();

        var bookings = await db.Bookings
            .Include(b => b.Screening)
            .Where(b => b.BookingTime >= from && b.BookingTime <= to && b.PaymentStatus == PaymentStatus.COMPLETED)
            .ToListAsync();

        var rows = movies
            .Select(m =>
            {
                var movieScreenings = screenings.Where(s => s.MovieId == m.Id).ToList();
                var movieBookings = bookings.Where(b => b.Screening?.MovieId == m.Id).ToList();
                return new
                {
                    movieTitle = m.Title,
                    genre = m.Genre.ToString(),
                    rating = m.Rating?.ToString() ?? "N/A",
                    screeningCount = movieScreenings.Count,
                    bookingCount = movieBookings.Count,
                    revenue = Math.Round(movieBookings.Sum(b => b.TotalAmount), 2),
                    seatsSold = movieBookings.Sum(b => b.BookedSeats.Count)
                };
            })
            .Where(r => r.screeningCount > 0 || r.bookingCount > 0)
            .OrderByDescending(r => r.revenue)
            .ToList();

        return new
        {
            rows,
            summary = new
            {
                totalMovies = rows.Count,
                totalRevenue = Math.Round(rows.Sum(r => r.revenue), 2),
                totalBookings = rows.Sum(r => r.bookingCount),
                totalScreenings = rows.Sum(r => r.screeningCount)
            }
        };
    }
}
