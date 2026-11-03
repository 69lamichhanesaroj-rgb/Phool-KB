using Infra;
using Infra.Entities;
using LinqToDB;
using Service.Security;

namespace Service;

public class UserService(AppDataConnection db, HashingPassword hp)
{
    public List<UserDto> GetUsers()
    {
        return (from u in db.Users join r in db.Roles on u.RoleId equals r.Id select new UserDto(u.Id, u.Username, r.RoleName)).ToList();
    }

    public void CreateUser(string username, int roleId, string password)
    {
        var passwordHashResult = hp.HashAndSaltPassword(password);
        var user = new User
        {
            Username = username,
            RoleId = roleId,
            PasswordHash = passwordHashResult.Hash,
            PasswordSalt = passwordHashResult.Salt
        };
        db.Insert(user);
    }
}
