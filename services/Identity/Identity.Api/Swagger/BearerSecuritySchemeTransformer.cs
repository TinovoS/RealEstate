using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Identity.Api.Swagger;

// Dodaje u OpenAPI dokument šemu "Bearer" (JWT u zaglavlju Authorization).
// Swagger UI na osnovu nje prikazuje dugme Authorize.
public class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public const string SchemeName = "Bearer";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var scheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT iz odgovora POST /api/auth/login (polje accessToken), bez prefiksa Bearer.",
        };

        document.Components ??= new OpenApiComponents();

        // Upisuje šemu u components.securitySchemes i registruje je u workspace dokumenta,
        // pa se reference iz operacija razrešavaju pri serijalizaciji.
        document.AddComponent<IOpenApiSecurityScheme>(SchemeName, scheme);

        return Task.CompletedTask;
    }
}
