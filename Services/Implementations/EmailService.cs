using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Services.Implementations;

public class EmailService(IConfiguration config, ILogger<EmailService> logger) : IEmailService
{
    private readonly string _host = config["Email:Host"] ?? "smtp.gmail.com";
    private readonly int _port = int.Parse(config["Email:Port"] ?? "587");
    private readonly string _username = config["Email:Username"] ?? string.Empty;
    private readonly string _password = config["Email:Password"] ?? string.Empty;
    private readonly string _fromName = config["Email:FromName"] ?? "Theatre MS";

    private async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
        {
            logger.LogWarning("Email not configured — skipping send to {Email}", toEmail);
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_fromName, _username));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var smtp = new SmtpClient();
        // Port 587 = STARTTLS, port 465 = SSL
        var socketOptions = _port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
        await smtp.ConnectAsync(_host, _port, socketOptions);
        await smtp.AuthenticateAsync(_username, _password);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }

    public Task SendPasswordResetEmailAsync(string toEmail, string resetToken) =>
        SendAsync(toEmail, "Password Reset Request",
            $"<p>Click the link to reset your password:</p>" +
            $"<a href='https://tms-fn.vercel.app/reset-password?token={resetToken}'>Reset Password</a>" +
            $"<p>This link expires in 1 hour.</p>");

    public Task SendOtpEmailAsync(string toEmail, string otp) =>
        SendAsync(toEmail, "Your Verification Code",
            $"<p>Your one-time verification code is:</p><h2>{otp}</h2><p>Valid for 5 minutes.</p>");

    public Task SendContactEmailAsync(string name, string fromEmail, string subject, string message) =>
        SendAsync(_username, $"Contact: {subject}",
            $"<p>From: {name} ({fromEmail})</p><p>{message}</p>");

    public Task SendBookingConfirmationAsync(string toEmail, string bookingNumber, string movieTitle, DateTime screeningTime) =>
        SendAsync(toEmail, "Booking Confirmation",
            $"<p>Your booking <strong>{bookingNumber}</strong> for <strong>{movieTitle}</strong> on {screeningTime:f} is confirmed!</p>");
}
