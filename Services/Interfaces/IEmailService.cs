namespace TheatreMs.Api.Services.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string resetToken);
    Task SendOtpEmailAsync(string toEmail, string otp);
    Task SendContactEmailAsync(string name, string fromEmail, string subject, string message);
    Task SendBookingConfirmationAsync(string toEmail, string bookingNumber, string movieTitle, DateTime screeningTime);
}
