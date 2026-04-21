using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheatreMs.Api.Common;
using TheatreMs.Api.DTOs.Contact;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Controllers;

[ApiController]
[Route("api")]
public class HomeController(IDashboardService dashboardService, IEmailService emailService) : ControllerBase
{
    [HttpGet("home")]
    public ActionResult<ApiResponse<string>> Home() =>
        Ok(ApiResponse<string>.Ok("Welcome to Theatre Management System"));

    [HttpGet("about")]
    public ActionResult<ApiResponse<Dictionary<string, string>>> About()
    {
        var info = new Dictionary<string, string>
        {
            ["name"] = "Theatre Management System",
            ["version"] = "1.0.0",
            ["description"] = "A full-featured theatre management platform"
        };
        return Ok(ApiResponse<Dictionary<string, string>>.Ok(info));
    }

    [HttpGet("contact")]
    public ActionResult<ApiResponse<string>> ContactInfo() =>
        Ok(ApiResponse<string>.Ok("Contact us at support@theatrems.com"));

    [HttpPost("contact")]
    public async Task<ActionResult<ApiResponse<string>>> Contact([FromBody] ContactRequestDto dto)
    {
        await emailService.SendContactEmailAsync(dto.Name, dto.Email, dto.Subject, dto.Message);
        return Ok(ApiResponse<string>.Ok("Message sent successfully"));
    }

    [HttpGet("dashboard")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<Dictionary<string, object>>>> Dashboard()
    {
        var userId = long.Parse(User.FindFirstValue("userId")!);
        var data = await dashboardService.GetUserDashboardAsync(userId);
        return Ok(ApiResponse<Dictionary<string, object>>.Ok(data));
    }
}
