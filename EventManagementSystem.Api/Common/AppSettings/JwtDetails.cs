namespace EventManagementSystem.Api.Common.AppSettings;

public class JwtDetails
{
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public string SecretKey { get; set; }
    public int ExpiryMinutes { get; set; }
}
