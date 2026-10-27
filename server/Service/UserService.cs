using Infra.Entities;
using Infra;

namespace Service;

public record UserDto(int Id, string Username, string Role);

public class UserService(AppDataConnection db)
{
    public List<UserDto> GetUsers()
    {
        return db.Users.Select(u => new UserDto(u.Id, u.Username, u.Role)).ToList();
    }
}
