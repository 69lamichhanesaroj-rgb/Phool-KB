namespace Service;

public record LoginRequest(string Username, string Password);
public record LoginResponse(string Token, int Id, string Username, string Role);

public record RoleDto(int Id, string RoleName);

public record UserDto(int Id, string Username, string Role);
public record CreateUserRequest(string Username, int RoleId, string Password);
