using Shared.Infrastructure.Configuration.OpenApi;
using Shared.Infrastructure.Database.Entities;
using SmartFormat;

namespace ReadOnlyDataService.Services;

/// <summary>
/// Represents a service for reading data from the database.
/// </summary>
/// <param name="clientFactory">Http client factory</param>
/// <param name="endpointProvider"></param>
internal sealed class ReadOnlyDatabaseService(
    IHttpClientFactory clientFactory,
    IEndpointProviderService endpointProvider) : IReadOnlyDatabaseService
{
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

        var openApiUri = await endpointProvider.GetEndpoint(searchTerm, cancellationToken);

        return openApiUri is null
            ? defaultValue
            : await operation.Invoke(openApiUri, cancellationToken);
    }
}
