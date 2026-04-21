using Microsoft.EntityFrameworkCore;
using TheatreMs.Api.Data;
using TheatreMs.Api.DTOs.Theatre;
using TheatreMs.Api.Models;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Services.Implementations;

public class TheatreService(AppDbContext db) : ITheatreService
{
    public async Task<List<TheatreDto>> GetAllAsync(string? search, string sortBy)
    {
        var q = db.Theatres.AsQueryable();
        if (!string.IsNullOrEmpty(search))
            q = q.Where(t => t.Name.Contains(search) || t.Address.Contains(search));
        q = sortBy.ToLower() == "name" ? q.OrderBy(t => t.Name) : q.OrderBy(t => t.Id);
        return await q.Select(t => MapToDto(t)).ToListAsync();
    }

    public async Task<TheatreDto?> GetByIdAsync(long id)
    {
        var t = await db.Theatres.FindAsync(id);
        return t == null ? null : MapToDto(t);
    }

    public async Task<TheatreDto> CreateAsync(TheatreDto dto)
    {
        var theatre = new Theatre
        {
            Name = dto.Name!, Address = dto.Address!, PhoneNumber = dto.PhoneNumber,
            Email = dto.Email, Description = dto.Description,
            TotalScreens = dto.TotalScreens, ImageUrl = dto.ImageUrl
        };
        db.Theatres.Add(theatre);
        await db.SaveChangesAsync();
        return MapToDto(theatre);
    }

    public async Task<TheatreDto> UpdateAsync(long id, TheatreDto dto)
    {
        var theatre = await db.Theatres.FindAsync(id) ?? throw new KeyNotFoundException("Theatre not found");
        if (dto.Name != null) theatre.Name = dto.Name;
        if (dto.Address != null) theatre.Address = dto.Address;
        if (dto.PhoneNumber != null) theatre.PhoneNumber = dto.PhoneNumber;
        if (dto.Email != null) theatre.Email = dto.Email;
        if (dto.Description != null) theatre.Description = dto.Description;
        if (dto.TotalScreens.HasValue) theatre.TotalScreens = dto.TotalScreens;
        if (dto.ImageUrl != null) theatre.ImageUrl = dto.ImageUrl;
        await db.SaveChangesAsync();
        return MapToDto(theatre);
    }

    public async Task DeleteAsync(long id)
    {
        var theatre = await db.Theatres.FindAsync(id) ?? throw new KeyNotFoundException("Theatre not found");
        db.Theatres.Remove(theatre);
        await db.SaveChangesAsync();
    }

    public async Task<object> GetTheatreSeatsAsync(long id)
    {
        var seats = await db.Seats.Where(s => s.TheatreId == id).ToListAsync();
        var screens = seats.GroupBy(s => s.ScreenNumber)
            .ToDictionary(g => g.Key, g => g.Select(s => new
            {
                id = s.Id, rowName = s.RowName, seatNumber = s.SeatNumber,
                seatType = s.SeatType.ToString(), priceMultiplier = s.PriceMultiplier
            }));
        return new { theatreId = id, screens };
    }

    public async Task InitializeSeatsAsync(long theatreId, int screenNumber, int rows, int seatsPerRow)
    {
        var existing = db.Seats.Where(s => s.TheatreId == theatreId && s.ScreenNumber == screenNumber);
        db.Seats.RemoveRange(existing);

        var newSeats = new List<Seat>();
        for (int r = 0; r < rows; r++)
        {
            var rowName = ((char)('A' + r)).ToString();
            for (int n = 1; n <= seatsPerRow; n++)
            {
                newSeats.Add(new Seat
                {
                    TheatreId = theatreId, ScreenNumber = screenNumber,
                    RowName = rowName, SeatNumber = n,
                    SeatType = SeatType.STANDARD, PriceMultiplier = 1.0
                });
            }
        }
        db.Seats.AddRange(newSeats);
        await db.SaveChangesAsync();
    }

    public async Task<object> GetTheatreScreeningsAsync(long id)
    {
        var theatre = await db.Theatres.FindAsync(id) ?? throw new KeyNotFoundException("Theatre not found");
        var screenings = await db.Screenings.Include(s => s.Movie)
            .Where(s => s.TheatreId == id).ToListAsync();
        return new
        {
            theatre = MapToDto(theatre),
            screenings = screenings.Select(s => new
            {
                id = s.Id, movieTitle = s.Movie?.Title, startTime = s.StartTime,
                screenNumber = s.ScreenNumber, format = s.Format.ToString(), basePrice = s.BasePrice
            })
        };
    }

    public static TheatreDto MapToDto(Theatre t) => new()
    {
        Id = t.Id, Name = t.Name, Address = t.Address, PhoneNumber = t.PhoneNumber,
        Email = t.Email, Description = t.Description, TotalScreens = t.TotalScreens, ImageUrl = t.ImageUrl
    };
}
