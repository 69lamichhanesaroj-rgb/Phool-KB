using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Security;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(UserService userService, RoleService roleService) : ControllerBase
{
    private string Permission => User.FindFirstValue(ClaimTypes.Role) ?? throw new UnauthorizedAccessException("Access denied: User role not found.");
    
    private int ActorId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : throw new UnauthorizedAccessException("Access denied: User ID not found.");
    
    [HttpGet]
    [Authorize]
    public List<UserDto> GetUsers()
    {
        return userService.GetUsers();
    }

    [HttpPost]
    [Authorize(Roles = "superuser,admin")]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        var role = roleService.GetRole(request.RoleId);
        if (role is null)
        {
            return BadRequest(new { message = "Invalid role ID" });
        }

        if (Permission == "superuser" && (role.RoleName == "admin" || role.RoleName == "superuser"))
        {
            return StatusCode(403, new { message = "Superusers cannot create users with admin or superuser roles." });
        }

        try
        {
            userService.CreateUser(request.Username, request.RoleId, request.Password);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "superuser,admin")]
    public IActionResult UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        var target = userService.GetUser(id);
        if (target is null)
        {
            return NotFound(new { message = "User not found" });
        }
        
        var newRole = roleService.GetRole(request.RoleId);
        if (newRole is null)
        {
            return BadRequest(new { message = "Invalid role ID" });
        }

        if (Permission == "superuser" && (target.Role != "user" || newRole.RoleName != "user"))
        {
            return StatusCode(403, new { message = "Invalid role change" });
        }

        try
        {
            userService.UpdateUser(id, request.Username, request.RoleId, request.Password);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "superuser,admin")]
    public IActionResult DeleteUser(int id)
    {
        var target = userService.GetUser(id);
        if (target is null)
        {
            return NotFound(new { message = "User not found" });
        }

        if (Permission == "superuser" && target.Role != "user")
        {
            return StatusCode(403, new { message = "Superusers can only delete users with the 'user' role." });
        }
        
        // Superusers and users cannot delete their own accounts by the previous check.
        // This checks in case an admin tries to delete their own account
        if (id == ActorId)
        {
            return BadRequest(new { message = "You cannot delete your own account." });
        }

        try
        {
            userService.DeleteUser(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}