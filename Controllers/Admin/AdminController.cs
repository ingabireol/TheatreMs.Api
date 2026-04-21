using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheatreMs.Api.Common;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Controllers.Admin;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "ROLE_ADMIN")]
public class AdminController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, object>>>> Dashboard()
    {
        var data = await dashboardService.GetAdminDashboardAsync();
        return Ok(ApiResponse<Dictionary<string, object>>.Ok(data));
    }
}
