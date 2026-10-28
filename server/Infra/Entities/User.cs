namespace Infra.Entities;

using LinqToDB.Mapping;

[Table(Name = "Users")]
public class User 
{
    [PrimaryKey, Identity]
    public int Id { get; set; }
    
    [Column(Name = "Username"), NotNull]
    public string Username { get; set; } = string.Empty;

    [Column(Name = "Role"), NotNull]
    public string Role { get; set; } = string.Empty;

    [Column(Name = "PasswordHash"), NotNull]
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

    [Column(Name = "PasswordSalt"), NotNull]
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
}