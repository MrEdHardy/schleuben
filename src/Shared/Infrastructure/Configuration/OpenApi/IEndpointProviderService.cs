namespace Shared.Infrastructure.Configuration.OpenApi;

/// <summary>
/// Endpoint provider service contract
/// </summary>
public interface IEndpointProviderService
{
    /// <summary>
    /// Initializes the endpoints.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task InitializeEndpoints(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves an endpoint by the specified search term.
    /// </summary>
    /// <param name="searchTerm">Endpoint search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="typeIdentifier">Optional type to categorize endpoints if required</param>
    /// <returns>The matched endpoint as an absolute uri or null if not found</returns>
    Task<Uri?> GetEndpoint(string searchTerm, CancellationToken cancellationToken, string? typeIdentifier = null);
}
