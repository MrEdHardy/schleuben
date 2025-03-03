using Microsoft.Extensions.Options;
using Shared.Infrastructure.Configuration;
using Shared.Infrastructure.Configuration.OpenApi;
using Timer = System.Timers.Timer;

namespace MutableDataService.Services;

/// <summary>
/// Default service for providing endpoints.
/// </summary>
/// <param name="logger">Logger</param>
/// <param name="optionsMonitor">Options monitor</param>
public sealed class DefaultEndpointProviderService(
    ILogger<DefaultEndpointProviderService> logger,
    IOptionsMonitor<IAddressSettings> optionsMonitor) : IEndpointProviderService
{
    private static readonly SemaphoreSlim Semaphore = new(1, 1);

    private readonly HashSet<string> endPoints = [];

    /// <inheritdoc/>
    public async Task InitializeEndpoints(CancellationToken cancellationToken)
    {
        if (this.endPoints.Count > 0)
        {
            return;
        }

        await Semaphore.WaitAsync(cancellationToken);

        try
        {
            await this.FetchEndpoints(cancellationToken);

            var timer = new Timer
            {
                Interval = TimeSpan.FromMinutes(5).TotalMilliseconds,
                AutoReset = true,
            };

            timer.Elapsed += async (_, _) =>
            {
                await Semaphore.WaitAsync();

                try
                {
                    logger.LogInformation("Refreshing endpoints.");

                    await this.FetchEndpoints(cancellationToken);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Failed to refresh endpoints.");
                }
                finally
                {
                    Semaphore.Release();
                }
            };

            timer.Start();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to initialize endpoints.");

            throw;
        }
        finally
        {
            Semaphore.Release();
        }
    }

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

        await Semaphore.WaitAsync(cancellationToken);

        try
        {
            _ = this.endPoints.TryGetValue(searchTerm, out string? result);

            if (result is not null)
            {
                return new Uri(this.GetUri("DatabaseService"), result);
            }

            string? containsSearch = this.endPoints
                .FirstOrDefault(ep => ep.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            return containsSearch is null
                ? null
                : new Uri(this.GetUri("DatabaseService"), containsSearch);
        }
        finally
        {
            Semaphore.Release();
        }
    }

    private async Task FetchEndpoints(CancellationToken cancellationToken = default)
    {
        this.endPoints.Clear();

        var uri = this.GetOpenApiUri();

        var endPointInfo = await OpenApiDefinitionResolver.ParseCapabilities(
            uri,
            cancellationToken);

        foreach (string endPoint in endPointInfo)
        {
            this.endPoints.Add(endPoint);
        }
    }

    private Uri GetOpenApiUri()
    {
        var baseUri = this.GetUri("DatabaseService");
        var openApiUri = this.GetUri("OpenApiPath");

        return new Uri(baseUri, openApiUri);
    }

    private Uri GetUri(string key)
    {
        var options = optionsMonitor.CurrentValue;

        _ = options.Addresses.TryGetValue(key, out var uri);

        if (uri is null)
        {
            const string message = "{key} is not a configured uri.";

            logger.LogError(message, key);

            throw new InvalidOperationException(message);
        }

        return uri;
    }
}
