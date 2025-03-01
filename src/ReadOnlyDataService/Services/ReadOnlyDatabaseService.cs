using Microsoft.Extensions.Options;
using ReadOnlyDataService.Configuration;
using Shared.Infrastructure.Configuration.OpenApi;
using Shared.Infrastructure.Database.Entities;
using SmartFormat;

namespace ReadOnlyDataService.Services;

/// <summary>
/// Represents a service for reading data from the database.
/// </summary>
/// <param name="clientFactory">Http client factory</param>
/// <param name="optionsMonitor">Options</param>
internal sealed class ReadOnlyDatabaseService(
    IHttpClientFactory clientFactory,
    IOptionsMonitor<ReadOnlyDataServiceOptions> optionsMonitor) : IReadOnlyDatabaseService
{
    private static readonly HashSet<string> paths = [];
    private static readonly SemaphoreSlim semaphore = new(1, 1);

    private static bool pathsInitialized;
    private static Uri? currentBaseUri;

    private readonly ReadOnlyDataServiceOptions options = optionsMonitor.CurrentValue;

    /// <inheritdoc/>
    public Task<AddressEntity?> GetAddressByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return this.GetById<AddressEntity>("GetAddressById", id, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IEnumerable<AddressEntity>> GetAddressesAsync(CancellationToken cancellationToken)
    {
        return this.GetMany<AddressEntity>("addresses", cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IEnumerable<PersonEntity>> GetPeopleAsync(CancellationToken cancellationToken)
    {
        return this.GetMany<PersonEntity>("people", cancellationToken);
    }

    /// <inheritdoc/>
    public Task<PersonEntity?> GetPersonByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return this.GetById<PersonEntity>("GetPersonById", id, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<TelephoneConnectionEntity?> GetTelephoneConnectionByIdAsync(
        uint id,
        CancellationToken cancellationToken)
    {
        return this.GetById<TelephoneConnectionEntity>("GetTelephoneConnectionById", id, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IEnumerable<TelephoneConnectionEntity>> GetTelephoneConnectionsAsync(
        CancellationToken cancellationToken)
    {
        return this.GetMany<TelephoneConnectionEntity>("telephone-connections", cancellationToken);
    }

    private static async Task InitializePaths(Uri baseUri, CancellationToken cancellationToken)
    {
        if (pathsInitialized)
        {
            return;
        }

        await semaphore.WaitAsync(cancellationToken);

        try
        {
            currentBaseUri = baseUri;

            var result = await OpenApiDefinitionResolver.ParseCapabilities(new Uri(
                baseUri,
                "/openapi/v1.json"), cancellationToken);

            foreach (string path in result)
            {
                paths.Add(path);
            }

            pathsInitialized = true;
        }
        finally
        {
            semaphore.Release();
        }
    }

    private static async Task CheckCurrentBaseUri(Uri? currentlyActiveUri, CancellationToken cancellationToken)
    {
        currentBaseUri ??= currentlyActiveUri;

        if (currentBaseUri != currentlyActiveUri)
        {
            currentBaseUri = currentlyActiveUri;
            pathsInitialized = false;

        }

        if (pathsInitialized || currentlyActiveUri is null)
        {
            return;
        }

        paths.Clear();

        await InitializePaths(currentlyActiveUri, cancellationToken);
    }

    private static string? GetPathFragment(string searchString)
    {
        return !pathsInitialized
            ? null
            : paths.FirstOrDefault(k => k.Contains(searchString, StringComparison.OrdinalIgnoreCase));
    }

    private Uri? GetBaseDatabaseServiceUri()
    {
        _ = this.options.Addresses.TryGetValue("DatabaseService", out var uri);

        return uri;
    }

    private Task<T?> GetById<T>(string searchTerm, uint id, CancellationToken cancellationToken)
        where T : class
    {
        return this.GetAsync(
            searchTerm,
            async (baseUri, ct) =>
            {
                string formattedPath = Smart.Format(baseUri.ToString(), new { id });

                using var client = clientFactory.CreateClient("schleuben");

                return await client.GetFromJsonAsync<T>(new Uri(formattedPath), ct);
            },
            default,
            cancellationToken);
    }

    private Task<IEnumerable<T>> GetMany<T>(string searchTerm, CancellationToken cancellationToken)
        where T : class
    {
        return this.GetAsync(
            searchTerm,
            async (baseUri, ct) =>
            {
                using var client = clientFactory.CreateClient("schleuben");

                return await client.GetFromJsonAsync<IEnumerable<T>>(baseUri, ct) ?? [];
            },
            [],
            cancellationToken);
    }

    private async Task<TResult> GetAsync<TResult>(
        string searchTerm,
        Func<Uri, CancellationToken, Task<TResult>> operation,
        TResult defaultValue,
        CancellationToken cancellationToken)
    {
        var uri = this.GetBaseDatabaseServiceUri();

        await CheckCurrentBaseUri(uri, cancellationToken);

        string? path = GetPathFragment(searchTerm);

        if (uri is null || path is null)
        {
            return defaultValue;
        }

        var fullUri = new Uri(uri, path);

        return await operation.Invoke(fullUri, cancellationToken);
    }
}
