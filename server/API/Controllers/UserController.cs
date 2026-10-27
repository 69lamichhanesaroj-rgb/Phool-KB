using Microsoft.AspNetCore.Mvc;
using Service;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(UserService _userService) : ControllerBase
{
    [HttpGet]
    public List<UserDto> GetUsers()
    {
        return _userService.GetUsers();
    }
}