using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TheatreMs.Api.Data;
using TheatreMs.Api.DTOs.Auth;
using TheatreMs.Api.Security;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Services.Implementations;

public class TwoFactorAuthService(AppDbContext db, JwtService jwt, IEmailService emailService, IMemoryCache cache) : ITwoFactorAuthService
{
    private static readonly Random Rng = new();

    public async Task<Dictionary<string, object>> InitiateAsync(LoginRequest request)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username)
            ?? throw new UnauthorizedAccessException("Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            throw new UnauthorizedAccessException("Invalid credentials");

        var otp = Rng.Next(100000, 999999).ToString();
        var cacheKey = $"otp:{user.Username}";
        cache.Set(cacheKey, otp, TimeSpan.FromMinutes(5));

        await emailService.SendOtpEmailAsync(user.Email, otp);

        return new Dictionary<string, object>
        {
            ["requires2FA"] = true,
            ["message"] = "OTP sent to your email",
            ["username"] = user.Username,
            ["email"] = user.Email
        };
    }

    public async Task<LoginResponse> VerifyAsync(OtpVerificationRequest request)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username)
            ?? throw new UnauthorizedAccessException("Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            throw new UnauthorizedAccessException("Invalid credentials");

        var cacheKey = $"otp:{user.Username}";
        if (!cache.TryGetValue(cacheKey, out string? storedOtp) || storedOtp != request.Otp)
            throw new InvalidOperationException("Invalid or expired OTP");

        cache.Remove(cacheKey);

        return new LoginResponse
        {
            Token = jwt.GenerateToken(user),
            Username = user.Username,
            Roles = user.Role.ToString()
        };
    }
}
