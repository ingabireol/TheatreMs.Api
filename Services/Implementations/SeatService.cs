using Microsoft.EntityFrameworkCore;
using TheatreMs.Api.Data;
using TheatreMs.Api.DTOs.Seat;
using TheatreMs.Api.Models;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Services.Implementations;

public class SeatService(AppDbContext db) : ISeatService
{
    public async Task<object> GetTheatreScreensAsync(long theatreId)
    {
        var seats = await db.Seats.Where(s => s.TheatreId == theatreId).ToListAsync();
        var screens = seats.GroupBy(s => s.ScreenNumber).Select(g => new
        {
            screenNumber = g.Key,
            totalSeats = g.Count(),
            rows = g.Select(s => s.RowName).Distinct().OrderBy(r => r).ToList()
        });
        return new { theatreId, screens };
    }

    public async Task<List<SeatDto>> GetScreenSeatsAsync(long theatreId, int screenNumber) =>
        await db.Seats
            .Where(s => s.TheatreId == theatreId && s.ScreenNumber == screenNumber)
            .Select(s => new SeatDto
            {
                Id = s.Id, RowName = s.RowName, SeatNumber = s.SeatNumber,
                ScreenNumber = s.ScreenNumber, SeatType = s.SeatType.ToString(),
                PriceMultiplier = s.PriceMultiplier
            }).ToListAsync();

    public async Task<string> InitializeScreenAsync(long theatreId, int screenNumber, int rows, int seatsPerRow)
    {
        var existing = db.Seats.Where(s => s.TheatreId == theatreId && s.ScreenNumber == screenNumber);
        db.Seats.RemoveRange(existing);

        var newSeats = new List<Seat>();
        for (int r = 0; r < rows; r++)
        {
            var rowName = ((char)('A' + r)).ToString();
            for (int n = 1; n <= seatsPerRow; n++)
                newSeats.Add(new Seat { TheatreId = theatreId, ScreenNumber = screenNumber, RowName = rowName, SeatNumber = n, SeatType = SeatType.STANDARD, PriceMultiplier = 1.0 });
        }
        db.Seats.AddRange(newSeats);
        await db.SaveChangesAsync();
        return $"Initialized {newSeats.Count} seats for screen {screenNumber}";
    }

    public async Task UpdateSeatAsync(long seatId, string seatType, double priceMultiplier)
    {
        var seat = await db.Seats.FindAsync(seatId) ?? throw new KeyNotFoundException("Seat not found");
        if (Enum.TryParse<SeatType>(seatType, true, out var st)) seat.SeatType = st;
        seat.PriceMultiplier = priceMultiplier;
        await db.SaveChangesAsync();
    }

    public async Task UpdateRowAsync(long theatreId, int screenNumber, string rowName, string seatType, double priceMultiplier)
    {
        var seats = await db.Seats
            .Where(s => s.TheatreId == theatreId && s.ScreenNumber == screenNumber && s.RowName == rowName)
            .ToListAsync();
        if (Enum.TryParse<SeatType>(seatType, true, out var st))
            foreach (var seat in seats) { seat.SeatType = st; seat.PriceMultiplier = priceMultiplier; }
        await db.SaveChangesAsync();
    }

    public async Task<string> BulkUpdateAsync(long theatreId, int screenNumber, string selection, string seatType, double priceMultiplier)
    {
        var seats = await db.Seats
            .Where(s => s.TheatreId == theatreId && s.ScreenNumber == screenNumber)
            .ToListAsync();

        if (Enum.TryParse<SeatType>(seatType, true, out var st))
        {
            var targets = selection.ToLower() == "all" ? seats : seats.Where(s => s.RowName == selection).ToList();
            foreach (var seat in targets) { seat.SeatType = st; seat.PriceMultiplier = priceMultiplier; }
        }
        await db.SaveChangesAsync();
        return "Bulk update completed";
    }

    public async Task<string> DeleteScreenAsync(long theatreId, int screenNumber)
    {
        var seats = db.Seats.Where(s => s.TheatreId == theatreId && s.ScreenNumber == screenNumber);
        db.Seats.RemoveRange(seats);
        await db.SaveChangesAsync();
        return $"Screen {screenNumber} deleted";
    }
}
