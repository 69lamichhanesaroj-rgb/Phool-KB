using Microsoft.AspNetCore.Mvc;
using Service;
using Service.DTOs;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(UserService _userService) : ControllerBase
{
    [HttpGet]
    public List<DisplayUsers> GetUsers()
    {
        return _userService.GetUsers();
    }
}