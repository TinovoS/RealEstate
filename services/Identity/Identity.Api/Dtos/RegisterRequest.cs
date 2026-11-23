using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Dtos;

// Podaci koje klijent šalje na POST /api/auth/register.
public class RegisterRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    // Pravila za jačinu lozinke proverava Identity (podešava se u Program.cs).
    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    // Mora biti jedna od vrednosti iz Roles.All; proverava kontroler.
    [Required]
    public string Role { get; set; } = string.Empty;
}
