using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheatreMs.Api.Common;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/reports")]
[Authorize(Roles = "ROLE_ADMIN,ROLE_MANAGER")]
public class ReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetReport(
        [FromQuery] string type = "bookings",
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var fromDate = from ?? DateTime.UtcNow.AddDays(-30);
        var toDate = (to ?? DateTime.UtcNow).Date.AddDays(1).AddSeconds(-1);

        var result = type.ToLower() switch
        {
            "revenue" => await reportService.GetRevenueReportAsync(fromDate, toDate),
            "movies" => await reportService.GetMoviePerformanceReportAsync(fromDate, toDate),
            _ => await reportService.GetBookingReportAsync(fromDate, toDate)
        };

        return Ok(ApiResponse<object>.Ok(result));
    }
}
