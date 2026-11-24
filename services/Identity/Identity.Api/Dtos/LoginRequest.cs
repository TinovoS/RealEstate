using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Dtos;

// Podaci koje klijent šalje na POST /api/auth/login.
public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
