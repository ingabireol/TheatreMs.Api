using Microsoft.EntityFrameworkCore;
using TheatreMs.Api.Data;
using TheatreMs.Api.Models;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Services.Implementations;

public class PasswordResetService(AppDbContext db, IEmailService emailService) : IPasswordResetService
{
    public async Task InitiateResetAsync(string email)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return; // Don't reveal whether email exists

        // Expire old tokens
        var oldTokens = db.PasswordResetTokens.Where(t => t.Email == email && !t.Used);
        db.PasswordResetTokens.RemoveRange(oldTokens);

        var token = new PasswordResetToken
        {
            Token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"),
            Email = email,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
        db.PasswordResetTokens.Add(token);
        await db.SaveChangesAsync();

        await emailService.SendPasswordResetEmailAsync(email, token.Token);
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        var record = await db.PasswordResetTokens
            .FirstOrDefaultAsync(t => t.Token == token && !t.Used && t.ExpiresAt > DateTime.UtcNow);
        return record != null;
    }

    public async Task ResetPasswordAsync(string token, string newPassword)
    {
        var record = await db.PasswordResetTokens
            .FirstOrDefaultAsync(t => t.Token == token && !t.Used && t.ExpiresAt > DateTime.UtcNow)
            ?? throw new InvalidOperationException("Invalid or expired token");

        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == record.Email)
            ?? throw new InvalidOperationException("User not found");

        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        record.Used = true;
        await db.SaveChangesAsync();
    }
}
