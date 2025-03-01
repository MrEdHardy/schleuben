using Microsoft.Extensions.Options;
using MutableDataService.Configuration;
using Shared.Infrastructure.Configuration.OpenApi;
using Timer = System.Timers.Timer;

namespace MutableDataService.Services;

/// <summary>
/// Service for providing endpoints.
/// </summary>
/// <param name="logger">Logger</param>
/// <param name="optionsMonitor">Options monitor</param>
public sealed class EndpointProviderService(
    ILogger<EndpointProviderService> logger,
    IOptionsMonitor<MutableDataServiceOptions> optionsMonitor)
{
    private static readonly SemaphoreSlim semaphore = new(1, 1);

    private readonly HashSet<string> endPoints = [];

    /// <summary>
    /// Initializes the endpoints.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    /// <exception cref="InvalidOperationException">
    /// If no uri or relative path to an open api document are configured.
    /// </exception>
    public async Task InitializeEndpoints(CancellationToken cancellationToken)
    {
        if (this.endPoints.Count > 0)
        {
            return;
        }

        await semaphore.WaitAsync(cancellationToken);

        try
        {
            await this.FetchEndpoints(cancellationToken);

            var timer = new Timer
            {
                Interval = TimeSpan.FromMinutes(1).TotalMilliseconds,
                AutoReset = true,
            };

            timer.Elapsed += async (_, _) =>
            {
                await semaphore.WaitAsync();

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
                    semaphore.Release();
                }
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to initialize endpoints.");
            throw;
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    /// Retrieves the endpoints.
    /// </summary>
    /// <returns>Collection of endpoints</returns>
    public async Task<IEnumerable<string>> GetEndPoints()
    {
        await semaphore.WaitAsync();

        try
        {
            return this.endPoints;
        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    /// Retrieves an endpoint by the specified search term.
    /// </summary>
    /// <param name="searchTerm">Endpoint search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The matched endpoint as an absolute uri or null if not found</returns>
    public async Task<Uri?> GetEndpoint(string searchTerm, CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);

        try
        {
            _ = this.endPoints.TryGetValue(searchTerm, out string? result);

            if (result is not null)
            {
                return new Uri(this.GetBaseUri(), result);
            }

            string? containsSearch = this.endPoints
                .FirstOrDefault(ep => ep.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            return containsSearch is null
                ? null
                : new Uri(this.GetBaseUri(), containsSearch);
        }
        finally
        {
            semaphore.Release();
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
        var options = optionsMonitor.CurrentValue;
        var uri = this.GetBaseUri();

        if (string.IsNullOrWhiteSpace(options.OpenApiPath))
        {
            const string message = "No relative path to open api spec is configured.";

            logger.LogError(message);

            throw new InvalidOperationException(message);
        }

        return new Uri(uri, options.OpenApiPath);
    }

    private Uri GetBaseUri()
    {
        var options = optionsMonitor.CurrentValue;

        _ = options.Addresses.TryGetValue("DatabaseService", out var uri);

        if (uri is null)
        {
            const string message = "DatabaseService URI is not configured.";

            logger.LogError(message);

            throw new InvalidOperationException(message);
        }

        return uri;
    }
}
