using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Dtos;

// Podaci koje klijent šalje na PUT /api/users/me. Mejl, lozinka i uloga se ovde ne menjaju.
public class UpdateProfileRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
}
