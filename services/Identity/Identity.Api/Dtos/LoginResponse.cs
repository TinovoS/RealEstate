namespace Identity.Api.Dtos;

// Podaci koje servis vraća posle uspešne prijave.
public class LoginResponse
{
    // JWT koji klijent šalje uz svaki sledeći zahtev, u zaglavlju Authorization.
    public string AccessToken { get; set; } = string.Empty;

    // Trenutak posle kog token više ne važi (UTC).
    public DateTime ExpiresAt { get; set; }
}
