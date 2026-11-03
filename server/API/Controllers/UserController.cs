using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(UserService userService) : ControllerBase
{
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
}