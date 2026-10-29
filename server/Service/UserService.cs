using Infra;
using Infra.Entities;
using LinqToDB;
using Service.DTOs;
using Service.Security;

namespace Service;

public class UserService(AppDataConnection db, HashingPassword hp)
{
    public List<UsersDto> GetUsers()
    {
        return db.Users.Select(u => new UsersDto(u.Id, u.Username, u.RoleId, u.PasswordHash, u.PasswordSalt)).ToList();
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
