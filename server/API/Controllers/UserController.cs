using Microsoft.AspNetCore.Mvc;
using Service;
using Service.DTOs;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(UserService userService) : ControllerBase
{
    [HttpGet]
    public List<UsersDto> GetUsers()
    {
        return userService.GetUsers();
    }

    [HttpPost]
    public void CreateUser(string username, int roleId, string password)
    {
        userService.CreateUser(username, roleId, password);
    }
}