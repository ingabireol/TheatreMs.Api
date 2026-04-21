using TheatreMs.Api.DTOs.Auth;
using TheatreMs.Api.DTOs.User;

namespace TheatreMs.Api.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<UserDto> SignupAsync(SignupRequest request);
}
