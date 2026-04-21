using Microsoft.AspNetCore.Mvc;
using TheatreMs.Api.Common;
using TheatreMs.Api.DTOs.Auth;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Controllers;

[ApiController]
[Route("api/auth/2fa")]
public class TwoFactorAuthController(ITwoFactorAuthService twoFactorService) : ControllerBase
{
    [HttpPost("initiate")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, object>>>> Initiate([FromBody] LoginRequest request)
    {
        try
        {
            var result = await twoFactorService.InitiateAsync(request);
            return Ok(ApiResponse<Dictionary<string, object>>.Ok(result, "OTP sent"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<Dictionary<string, object>>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            // Typically an SMTP failure — credentials are valid but email couldn't be sent
            return StatusCode(500, ApiResponse<Dictionary<string, object>>.Fail($"Failed to send OTP: {ex.Message}"));
        }
    }

    [HttpPost("verify")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Verify([FromBody] OtpVerificationRequest request)
    {
        try
        {
            var result = await twoFactorService.VerifyAsync(request);
            return Ok(ApiResponse<LoginResponse>.Ok(result, "Verification successful"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<LoginResponse>.Fail(ex.Message));
        }
    }
}
