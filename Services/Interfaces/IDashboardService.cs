namespace TheatreMs.Api.Services.Interfaces;

public interface IDashboardService
{
    Task<Dictionary<string, object>> GetAdminDashboardAsync();
    Task<Dictionary<string, object>> GetUserDashboardAsync(long userId);
}
