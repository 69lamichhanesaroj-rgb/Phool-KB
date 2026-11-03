namespace Service.Security;

public class JwtOptions
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "Phool-KB";
    public string Audience { get; set; } = "Phool-KB-Client";
    public int ExpiryMinutes { get; set; } = 15;
}