namespace Service.DTOs;

public record UsersDto(int Id, string Username, int Role, byte[] PasswordHash, byte[] PasswordSalt);