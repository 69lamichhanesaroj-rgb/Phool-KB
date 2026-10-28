using Infra;
using Service.DTOs;

namespace Service;

public class UserService(AppDataConnection db)
{
    public List<DisplayUsers> GetUsers()
    {
        return db.Users.Select(u => new DisplayUsers(u.Id, u.Username, u.Role)).ToList();
    }
}
