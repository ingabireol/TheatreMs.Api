using Microsoft.AspNetCore.Mvc;
using TheatreMs.Api.Common;
using TheatreMs.Api.DTOs.Auth;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Controllers;

[ApiController]
[Route("api/auth/password")]
public class PasswordResetController(IPasswordResetService passwordResetService) : ControllerBase
{
    [HttpPost("forgot")]
    public async Task<ActionResult<ApiResponse<object>>> ForgotPassword([FromBody] PasswordResetRequest request)
    {
        try
        {
            await passwordResetService.InitiateResetAsync(request.Email);
            return Ok(ApiResponse<object>.Ok("If your email is registered, you will receive a reset link."));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail($"Failed to send email: {ex.Message}"));
        }
    }

    [HttpPost("validate-token")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, object>>>> ValidateToken([FromBody] PasswordResetTokenRequest request)
    {
        var valid = await passwordResetService.ValidateTokenAsync(request.Token);
        var result = new Dictionary<string, object> { ["valid"] = valid };
        return Ok(ApiResponse<Dictionary<string, object>>.Ok(result));
    }

    [HttpPost("reset")]
    public async Task<ActionResult<ApiResponse<object>>> ResetPassword([FromBody] PasswordResetTokenRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.NewPassword))
                return BadRequest(ApiResponse<object>.Fail("New password is required"));
            await passwordResetService.ResetPasswordAsync(request.Token, request.NewPassword);
            return Ok(ApiResponse<object>.Ok("Password reset successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
