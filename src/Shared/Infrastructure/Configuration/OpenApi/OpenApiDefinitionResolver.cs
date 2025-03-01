using NSwag;

namespace Shared.Infrastructure.Configuration.OpenApi;

/// <summary>
/// Represents a resolver for OpenAPI definitions.
/// </summary>
public static class OpenApiDefinitionResolver
{
    /// <summary>
    /// Parses the capabilities from the OpenAPI specification.
    /// </summary>
    /// <param name="uriToOpenApiSpec">Target uri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary</returns>
    public static async Task<HashSet<string>> ParseCapabilities(
        Uri uriToOpenApiSpec,
        CancellationToken cancellationToken)
    {
        var document = await OpenApiDocument.FromUrlAsync(uriToOpenApiSpec.AbsoluteUri, cancellationToken);

        return [.. document.Paths.Keys];
    }
}
