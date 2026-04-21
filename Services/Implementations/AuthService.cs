using Microsoft.EntityFrameworkCore;
using TheatreMs.Api.Data;
using TheatreMs.Api.DTOs.Auth;
using TheatreMs.Api.DTOs.User;
using TheatreMs.Api.Models;
using TheatreMs.Api.Security;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Services.Implementations;

public class AuthService(AppDbContext db, JwtService jwt) : IAuthService
{
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username)
            ?? throw new UnauthorizedAccessException("Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            throw new UnauthorizedAccessException("Invalid credentials");

        return new LoginResponse
        {
            Token = jwt.GenerateToken(user),
            Username = user.Username,
            Roles = user.Role.ToString()
        };
    }

    public async Task<UserDto> SignupAsync(SignupRequest request)
    {
        if (await db.Users.AnyAsync(u => u.Username == request.Username))
            throw new InvalidOperationException("Username is already taken");

        if (await db.Users.AnyAsync(u => u.Email == request.Email))
            throw new InvalidOperationException("Email is already in use");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Role = UserRole.ROLE_USER
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return MapToDto(user);
    }

    public static UserDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        PhoneNumber = user.PhoneNumber,
        Role = user.Role
    };
}
