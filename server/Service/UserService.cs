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

    public UserDto? GetUser(int id)
    {
        return (from u in db.Users join r in db.Roles on u.RoleId equals r.Id where u.Id == id select new UserDto(u.Id, u.Username, r.RoleName)).SingleOrDefault();
    }

    public void CreateUser(string username, int roleId, string password)
    {
        if (db.Users.Any(u => u.Username == username))
        {
            throw new InvalidOperationException("Username already exists.");
        }
        
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

    public void UpdateUser(int id, string username, int roleId, string password)
    {
        var user = db.Users.SingleOrDefault(u => u.Id == id) ?? throw new InvalidOperationException("Username not found");

        if (db.Users.Any(u => u.Username == username && u.Id != id)) throw new InvalidOperationException("Username already exists");

        user.Username = username;
        user.RoleId = roleId;

        if (!string.IsNullOrEmpty(password))
        {
            var hash = hp.HashAndSaltPassword(password);
            user.PasswordHash = hash.Hash;
            user.PasswordSalt = hash.Salt;
        }
        
        db.Update(user);
    }
    
    public void DeleteUser(int id)
    {
        var user = db.Users.SingleOrDefault(u => u.Id == id) ?? throw new InvalidOperationException("User not found.");

        db.Delete(user);
    }
}
