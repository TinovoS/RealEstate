using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Identity.Api.Swagger;

// Operaciji zaštićenoj sa [Authorize] (na metodi ili klasi), a bez [AllowAnonymous], dodaje zahtev za šemu "Bearer".
// Swagger UI tada šalje token samo uz te operacije i prikazuje katanac pored njih.
public class BearerSecurityRequirementTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;

        var requiresAuthorization = metadata.OfType<IAuthorizeData>().Any()
            && !metadata.OfType<IAllowAnonymous>().Any();

        if (requiresAuthorization)
        {
            operation.Security ??= [];
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(BearerSecuritySchemeTransformer.SchemeName, context.Document)] = [],
            });
        }

        return Task.CompletedTask;
    }
}
