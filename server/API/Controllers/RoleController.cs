using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Security;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoleController(RoleService roleService) : ControllerBase
{
    [HttpGet]
    public List<RoleDto> GetRoles()
    {
        return roleService.GetRoles();
    }
}