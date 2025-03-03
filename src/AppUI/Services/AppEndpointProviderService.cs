using AppUI.Configuration;
using Microsoft.Extensions.Options;
using Shared.Infrastructure.Configuration.OpenApi;

namespace AppUI.Services;

/// <summary>
/// App discoverable endpoint provider
/// </summary>
/// <param name="optionsMonitor"></param>
/// <param name="logger"></param>
public class AppEndpointProviderService(
    IOptionsMonitor<AppSettings> optionsMonitor,
    ILogger<AppEndpointProviderService> logger) : IEndpointProviderService
{
    private static readonly SemaphoreSlim SemaphoreSlim = new(1, 1);

    private readonly Dictionary<string, Uri> endPoints = [];

    /// <inheritdoc/>
    public async Task<Uri?> GetEndpoint(
        string searchTerm,
        CancellationToken cancellationToken,
        string? typeIdentifier = null)
    {
        if (this.endPoints.Count <= 0)
        {
            await this.InitializeEndpoints(cancellationToken);
        }

        await SemaphoreSlim.WaitAsync(cancellationToken);

        try
        {
            if (string.IsNullOrEmpty(typeIdentifier))
            {
                return null;
            }

            _ = this.endPoints.TryGetValue(typeIdentifier + '_' + searchTerm, out var directParse);

            if (directParse is not null)
            {
                return this.GetFullUri(typeIdentifier, directParse);
            }

            string? containsSearch = this.endPoints.Keys
                .FirstOrDefault(k => k.Contains(typeIdentifier, StringComparison.OrdinalIgnoreCase)
                    && k.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            return containsSearch is null
                ? null
                : this.GetFullUri(typeIdentifier, this.endPoints[containsSearch]);
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }

    /// <inheritdoc/>
    public async Task InitializeEndpoints(CancellationToken cancellationToken)
    {
        if (this.endPoints.Count > 0)
        {
            return;
        }

        await SemaphoreSlim.WaitAsync(cancellationToken);

        try
        {
            logger.LogInformation("Initializing endpoints");

            await this.RetrieveAllEndpointDefinitions(cancellationToken);

            var timer = new System.Timers.Timer
            {
                Interval = TimeSpan.FromMinutes(5).TotalMilliseconds,
                AutoReset = true,
            };

            timer.Elapsed += async (_, _) =>
            {
                await SemaphoreSlim.WaitAsync(cancellationToken);

                try
                {
                    logger.LogInformation("Refreshing discoverable endpoints");

                    await this.RetrieveAllEndpointDefinitions(cancellationToken);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Failed to refresh endpoints");
                }
                finally
                {
                    SemaphoreSlim.Release();
                }
            };

            timer.Start();
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }

    private async Task RetrieveAllEndpointDefinitions(CancellationToken cancellationToken)
    {
        const string readOnly = "ReadOnlyService";
        const string mutable = "MutableService";

        var readOnlyEndpoints = await this.FetchEndpoints(readOnly, cancellationToken);
        var mutableEndpoints = await this.FetchEndpoints(mutable, cancellationToken);

        foreach (string readOnlyEndpoint in readOnlyEndpoints)
        {
            this.endPoints.Add(readOnly + '_' + readOnlyEndpoint, new Uri(readOnlyEndpoint, UriKind.Relative));
        }

        foreach (string mutableEndpoint in mutableEndpoints)
        {
            this.endPoints.Add(mutable + '_' + mutableEndpoint, new Uri(mutableEndpoint, UriKind.Relative));
        }
    }

    private Task<HashSet<string>> FetchEndpoints(string endpointName, CancellationToken cancellationToken)
    {
        var uri = this.GetOpenApiUri(endpointName);

        return OpenApiDefinitionResolver.ParseCapabilities(uri, cancellationToken);
    }

    private Uri GetOpenApiUri(string endpointName)
    {
        var options = optionsMonitor.CurrentValue;

        var baseUri = this.GetBaseUri(endpointName);
        _ = options.Addresses.TryGetValue(endpointName + "_OpenApiPath", out var openApiUri);

        return openApiUri is null
            ? throw new InvalidOperationException($"No endpoint could be resolved for {openApiUri}")
            : new Uri(baseUri, openApiUri);
    }

    private Uri GetBaseUri(string endpointName)
    {
        var options = optionsMonitor.CurrentValue;

        _ = options.Addresses.TryGetValue(endpointName, out var baseEndpoint);

        return baseEndpoint is null
            ? throw new InvalidOperationException($"No endpoint could be resolved for {endpointName}")
            : baseEndpoint;
    }

    private Uri GetFullUri(string endpointName, Uri relativeUri)
    {
        return new Uri(this.GetBaseUri(endpointName), relativeUri);
    }
}
