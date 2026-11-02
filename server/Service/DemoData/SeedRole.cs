using Infra;
using Infra.Entities;
using LinqToDB;

namespace Service;

public class SeedRole(AppDataConnection db)
{
    private void Create(string roleName)
    {
        if (db.Roles.Any(r => r.RoleName == roleName))
        {
            return;
        }
        
        db.Insert(new Role {RoleName = roleName});
    }

    public void SeedRoles()
    {
        Create("user");
        Create("superuser");
        Create("admin");
    }
}