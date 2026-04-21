using Microsoft.EntityFrameworkCore;
using TheatreMs.Api.Data;
using TheatreMs.Api.DTOs.User;
using TheatreMs.Api.Models;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Services.Implementations;

public class UserService(AppDbContext db) : IUserService
{
    public async Task<List<UserDto>> GetAllAsync(string? search, string? role, string sortBy, string sortOrder)
    {
        var q = BuildUserQuery(search, role, sortBy, sortOrder);
        var users = await q.ToListAsync();
        return users.Select(MapToDto).ToList();
    }

    public async Task<(List<UserDto> Items, int TotalCount)> GetPagedAsync(string? search, string? role, string sortBy, string sortOrder, int page, int size)
    {
        var q = BuildUserQuery(search, role, sortBy, sortOrder);
        var total = await q.CountAsync();
        var users = await q.Skip(page * size).Take(size).ToListAsync();
        return (users.Select(MapToDto).ToList(), total);
    }

    private IQueryable<User> BuildUserQuery(string? search, string? role, string sortBy, string sortOrder)
    {
        var q = db.Users.AsQueryable();
        if (!string.IsNullOrEmpty(search))
            q = q.Where(u => u.Username.Contains(search) || u.Email.Contains(search) || u.FirstName.Contains(search) || u.LastName.Contains(search));
        if (!string.IsNullOrEmpty(role) && Enum.TryParse<UserRole>(role, true, out var r))
            q = q.Where(u => u.Role == r);

        return (sortBy.ToLower(), sortOrder.ToLower()) switch
        {
            ("email", _) => sortOrder == "desc" ? q.OrderByDescending(u => u.Email) : q.OrderBy(u => u.Email),
            ("role", _) => sortOrder == "desc" ? q.OrderByDescending(u => u.Role) : q.OrderBy(u => u.Role),
            ("username", "desc") => q.OrderByDescending(u => u.Username),
            _ => q.OrderBy(u => u.Username)
        };
    }

    public async Task<object?> GetUserWithBookingsAsync(long id)
    {
        var user = await db.Users.FindAsync(id);
        if (user == null) return null;
        var bookings = await db.Bookings
            .Include(b => b.Screening).ThenInclude(s => s.Movie)
            .Where(b => b.UserId == id)
            .Select(b => new { b.Id, b.BookingNumber, b.TotalAmount, b.PaymentStatus, b.BookingTime, movieTitle = b.Screening.Movie.Title })
            .ToListAsync();
        return new { user = MapToDto(user), bookings };
    }

    public async Task<UserDto> CreateAsync(UserDto dto)
    {
        if (await db.Users.AnyAsync(u => u.Username == dto.Username))
            throw new InvalidOperationException("Username already taken");
        if (await db.Users.AnyAsync(u => u.Email == dto.Email))
            throw new InvalidOperationException("Email already in use");
        var user = new User
        {
            Username = dto.Username!, Email = dto.Email!,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password ?? "changeme"),
            FirstName = dto.FirstName ?? string.Empty, LastName = dto.LastName ?? string.Empty,
            PhoneNumber = dto.PhoneNumber, Role = dto.Role ?? UserRole.ROLE_USER
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return MapToDto(user);
    }

    public async Task<UserDto> UpdateAsync(long id, UserDto dto)
    {
        var user = await db.Users.FindAsync(id) ?? throw new KeyNotFoundException("User not found");
        if (dto.FirstName != null) user.FirstName = dto.FirstName;
        if (dto.LastName != null) user.LastName = dto.LastName;
        if (dto.PhoneNumber != null) user.PhoneNumber = dto.PhoneNumber;
        if (dto.Email != null) user.Email = dto.Email;
        if (dto.Password != null) user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        await db.SaveChangesAsync();
        return MapToDto(user);
    }

    public async Task<UserDto> UpdateRoleAsync(long id, string role)
    {
        var user = await db.Users.FindAsync(id) ?? throw new KeyNotFoundException("User not found");
        if (!Enum.TryParse<UserRole>(role, true, out var r))
            throw new InvalidOperationException("Invalid role");
        user.Role = r;
        await db.SaveChangesAsync();
        return MapToDto(user);
    }

    public async Task DeleteAsync(long id)
    {
        var user = await db.Users.FindAsync(id) ?? throw new KeyNotFoundException("User not found");
        db.Users.Remove(user);
        await db.SaveChangesAsync();
    }

    public async Task<object> GetStatsAsync()
    {
        var total = await db.Users.CountAsync();
        var regular = await db.Users.CountAsync(u => u.Role == UserRole.ROLE_USER);
        var managers = await db.Users.CountAsync(u => u.Role == UserRole.ROLE_MANAGER);
        var admins = await db.Users.CountAsync(u => u.Role == UserRole.ROLE_ADMIN);
        return new { totalUsers = total, regularUsers = regular, managerUsers = managers, adminUsers = admins };
    }

    public async Task<bool> UsernameExistsAsync(string username) =>
        await db.Users.AnyAsync(u => u.Username == username);

    public async Task<bool> EmailExistsAsync(string email) =>
        await db.Users.AnyAsync(u => u.Email == email);

    public static UserDto MapToDto(User u) => new()
    {
        Id = u.Id, Username = u.Username, Email = u.Email,
        FirstName = u.FirstName, LastName = u.LastName,
        PhoneNumber = u.PhoneNumber, Role = u.Role
    };
}
