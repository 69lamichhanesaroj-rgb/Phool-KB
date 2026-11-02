using Infra;
using Infra.Entities;
using LinqToDB;
using Service.Security;

namespace Service;

public class SeedUser (AppDataConnection db, HashingPassword hp)
{
    private void Create(string username, string roleName, string password)
    {
        if (db.Users.Any(u => u.Username == username))
        {
            return;
        }
        
        var role = db.Roles.SingleOrDefault(r => r.RoleName == roleName);

        var hash = hp.HashAndSaltPassword(password);
        
        db.Insert(new User
        {
            Username = username,
            RoleId = role.Id,
            PasswordHash = hash.Hash,
            PasswordSalt = hash.Salt
        });
    }
    
    public void SeedUsers()
    {
        Create("admin", "admin", "admin123");
        Create("superuser", "superuser", "superuser123");
        Create("user", "user", "user123");
    }
}