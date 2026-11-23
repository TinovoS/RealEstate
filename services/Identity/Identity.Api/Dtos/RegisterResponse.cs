namespace Identity.Api.Dtos;

// Podaci koje servis vraća posle uspešne registracije. Bez lozinke i heša.
public class RegisterResponse
{
    public string Id { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}
