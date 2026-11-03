using Infra;

namespace Service.Security;

public class RoleService(AppDataConnection db)
{
    public List<RoleDto> GetRoles()
    {
        return db.Roles.Select(r => new RoleDto(r.Id, r.RoleName)).ToList();
    }
}