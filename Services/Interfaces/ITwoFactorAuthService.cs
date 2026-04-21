using TheatreMs.Api.DTOs.Auth;

namespace TheatreMs.Api.Services.Interfaces;

public interface ITwoFactorAuthService
{
    Task<Dictionary<string, object>> InitiateAsync(LoginRequest request);
    Task<LoginResponse> VerifyAsync(OtpVerificationRequest request);
}
