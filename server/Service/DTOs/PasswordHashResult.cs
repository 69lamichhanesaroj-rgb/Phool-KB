namespace Service.DTOs;

public record PasswordHashResult(byte[] Hash, byte[] Salt);