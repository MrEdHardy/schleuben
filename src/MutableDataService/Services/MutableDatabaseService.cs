using Shared.Infrastructure.Database.Entities;
using SmartFormat;

namespace MutableDataService.Services;

/// <summary>
/// Represents a service for managing mutable data.
/// </summary>
public sealed class MutableDatabaseService(
    IHttpClientFactory clientFactory,
    EndpointProviderService endpointProvider) : IMutableDatabaseService
{
    /// <inheritdoc/>
    public Task<AddressEntity> CreateAddressAsync(AddressEntity address, CancellationToken cancellationToken)
    {
        return this.Create(address, "CreateAddress", cancellationToken);
    }

    /// <inheritdoc/>
    public Task<PersonEntity> CreatePersonAsync(PersonEntity person, CancellationToken cancellationToken)
    {
        return this.Create(person, "CreatePerson", cancellationToken);
    }

    /// <inheritdoc/>
    public Task<TelephoneConnectionEntity> CreateTelephoneConnectionAsync(
        TelephoneConnectionEntity telephoneConnection,
        CancellationToken cancellationToken)
    {
        return this.Create(telephoneConnection, "CreateTelephoneConnection", cancellationToken);
    }

    /// <inheritdoc/>
    public Task DeleteAddressAsync(uint id, CancellationToken cancellationToken)
    {
        return this.Delete(id, "DeleteAddress", cancellationToken);
    }

    /// <inheritdoc/>
    public Task DeletePersonAsync(uint id, CancellationToken cancellationToken)
    {
        return this.Delete(id, "DeletePerson", cancellationToken);
    }

    /// <inheritdoc/>
    public Task DeleteTelephoneConnectionAsync(uint id, CancellationToken cancellationToken)
    {
        return this.Delete(id, "DeleteTelephoneConnection", cancellationToken);
    }

    /// <inheritdoc/>
    public Task UpdateAddressAsync(AddressEntity address, CancellationToken cancellationToken)
    {
        return this.Update(address, "UpdateAddress", cancellationToken);
    }

    /// <inheritdoc/>
    public Task UpdatePersonAsync(PersonEntity person, CancellationToken cancellationToken)
    {
        return this.Update(person, "UpdatePerson", cancellationToken);
    }

    /// <inheritdoc/>
    public Task UpdateTelephoneConnectionAsync(TelephoneConnectionEntity telephoneConnection, CancellationToken cancellationToken)
    {
        return this.Update(telephoneConnection, "UpdateTelephoneConnection", cancellationToken);
    }

    private async Task<T> Create<T>(T entity, string searchTerm, CancellationToken cancellationToken)
    {
        var endpoint = await this.GetEndpoint(searchTerm, cancellationToken);

        using var client = clientFactory.CreateClient("schleuben");

        var response = await client.PutAsJsonAsync(endpoint, entity, cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken)
            ?? throw new ArgumentException($"Create result for {typeof(T)} failed.");
    }

    private async Task Update<T>(T entity, string searchTerm, CancellationToken cancellationToken)
    {
        var endpoint = await this.GetEndpoint(searchTerm, cancellationToken);

        using var client = clientFactory.CreateClient("schleuben");

        var response = await client.PatchAsJsonAsync(endpoint, entity, cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    private async Task Delete(uint id, string searchTerm, CancellationToken cancellationToken)
    {
        var endpoint = await this.GetEndpoint(searchTerm, cancellationToken);

        using var client = clientFactory.CreateClient("schleuben");

        string formattedEndpoint = Smart.Format(endpoint.ToString(), new { id });

        var response = await client.DeleteAsync(formattedEndpoint, cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    private async Task<Uri> GetEndpoint(string searchTerm, CancellationToken cancellationToken)
    {
        var endpoint = await endpointProvider.GetEndpoint(searchTerm, cancellationToken)
            ?? throw new ArgumentException("Endpoint couldn't be determined!");

        return endpoint;
    }
}
