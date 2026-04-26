using Microsoft.EntityFrameworkCore;
using TheatreMs.Api.Models;

namespace TheatreMs.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Theatre> Theatres => Set<Theatre>();
    public DbSet<Screening> Screenings => Set<Screening>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Username).IsUnique();
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Role).HasConversion<string>();
        });

        modelBuilder.Entity<Movie>(e =>
        {
            e.Property(m => m.Genre).HasConversion<string>();
            e.Property(m => m.Rating).HasConversion<string>();
        });

        modelBuilder.Entity<Screening>(e =>
        {
            e.Property(s => s.Format).HasConversion<string>();
        });

        modelBuilder.Entity<Seat>(e =>
        {
            e.Property(s => s.SeatType).HasConversion<string>();
        });

        modelBuilder.Entity<Booking>(e =>
        {
            e.Property(b => b.PaymentStatus).HasConversion<string>();
            // Store booked seats as JSON
            e.Property(b => b.BookedSeats)
             .HasColumnType("text")
             .HasConversion(
                 v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                 v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>()
             )
             .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
                 (l1, l2) => l1!.SequenceEqual(l2!),
                 l => l.Aggregate(0, (h, s) => HashCode.Combine(h, s.GetHashCode())),
                 l => l.ToList()
             ));
        });
    }
}
