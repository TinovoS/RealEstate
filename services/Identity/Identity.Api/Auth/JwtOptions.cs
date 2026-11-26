using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Auth;

// Podešavanja iz sekcije "Jwt" u appsettings fajlovima i environment varijablama.
public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    // Tajni ključ za potpis. Za HMAC-SHA256 mora imati najmanje 32 bajta.
    [Required]
    [MinLength(32)]
    public string Key { get; set; } = string.Empty;

    [Range(1, 1440)]
    public int ExpiresInMinutes { get; set; }
}
