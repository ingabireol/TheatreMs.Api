using TheatreMs.Api.DTOs.User;

namespace TheatreMs.Api.Services.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync(string? search, string? role, string sortBy, string sortOrder);
    Task<(List<UserDto> Items, int TotalCount)> GetPagedAsync(string? search, string? role, string sortBy, string sortOrder, int page, int size);
    Task<object?> GetUserWithBookingsAsync(long id);
    Task<UserDto> CreateAsync(UserDto dto);
    Task<UserDto> UpdateAsync(long id, UserDto dto);
    Task<UserDto> UpdateRoleAsync(long id, string role);
    Task DeleteAsync(long id);
    Task<object> GetStatsAsync();
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);
}
