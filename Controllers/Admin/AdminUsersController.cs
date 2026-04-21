using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheatreMs.Api.Common;
using TheatreMs.Api.DTOs.User;
using TheatreMs.Api.Services.Interfaces;

namespace TheatreMs.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "ROLE_ADMIN")]
public class AdminUsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetAll(
        [FromQuery] string? search, [FromQuery] string? role,
        [FromQuery] string sortBy = "username", [FromQuery] string sortOrder = "asc")
    {
        var users = await userService.GetAllAsync(search, role, sortBy, sortOrder);
        return Ok(ApiResponse<List<UserDto>>.Ok(users));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<object>>> GetById(long id)
    {
        var result = await userService.GetUserWithBookingsAsync(id);
        if (result == null) return NotFound(ApiResponse<object>.Fail("User not found"));
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserDto>>> Create([FromBody] UserDto dto)
    {
        try
        {
            var user = await userService.CreateAsync(dto);
            return Ok(ApiResponse<UserDto>.Ok(user, "User created"));
        }
        catch (InvalidOperationException ex) { return BadRequest(ApiResponse<UserDto>.Fail(ex.Message)); }
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Update(long id, [FromBody] UserDto dto)
    {
        try
        {
            var user = await userService.UpdateAsync(id, dto);
            return Ok(ApiResponse<UserDto>.Ok(user, "User updated"));
        }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<UserDto>.Fail(ex.Message)); }
    }

    [HttpPut("{id:long}/role")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateRole(long id, [FromBody] Dictionary<string, string> body)
    {
        try
        {
            var role = body.GetValueOrDefault("role") ?? throw new InvalidOperationException("Role is required");
            var user = await userService.UpdateRoleAsync(id, role);
            return Ok(ApiResponse<UserDto>.Ok(user, "Role updated"));
        }
        catch (Exception ex) { return BadRequest(ApiResponse<UserDto>.Fail(ex.Message)); }
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(long id)
    {
        try
        {
            await userService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok("User deleted"));
        }
        catch (KeyNotFoundException ex) { return NotFound(ApiResponse<object>.Fail(ex.Message)); }
    }

    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<object>>> GetStats()
    {
        var stats = await userService.GetStatsAsync();
        return Ok(ApiResponse<object>.Ok(stats));
    }

    [HttpGet("check-username")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, bool>>>> CheckUsername([FromQuery] string username)
    {
        var exists = await userService.UsernameExistsAsync(username);
        return Ok(ApiResponse<Dictionary<string, bool>>.Ok(new Dictionary<string, bool> { ["exists"] = exists }));
    }

    [HttpGet("check-email")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, bool>>>> CheckEmail([FromQuery] string email)
    {
        var exists = await userService.EmailExistsAsync(email);
        return Ok(ApiResponse<Dictionary<string, bool>>.Ok(new Dictionary<string, bool> { ["exists"] = exists }));
    }
}
