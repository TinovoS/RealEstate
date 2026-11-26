namespace Identity.Api.Dtos;

// Javni profil drugog korisnika, za GET /api/users/{id}. Bez mejla.
public class PublicProfileResponse
{
    public string Id { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}
