using Shared.Infrastructure.Configuration.Json;
using Shared.Infrastructure.Configuration.OpenApi;
using Shared.Infrastructure.Database.Entities;
using SmartFormat;
using System.Text.Json;

namespace AppUI.Services;

public sealed class MinimalDataService(
    IEndpointProviderService endpointProvider,
    IHttpClientFactory clientFactory,
    JsonSerializerOptionsProvider optionsProvider)
{
    public async Task<List<PersonEntity>> GetAllPeople(CancellationToken cancellationToken)
    {
        using var client = this.GetHttpClient();

        var uri = await endpointProvider.GetEndpoint("people", cancellationToken, "ReadOnlyService");

        return await client.GetFromJsonAsync<List<PersonEntity>>(uri, cancellationToken) ?? [];
    }

    public async Task UpdatePerson(PersonEntity person, CancellationToken cancellationToken)
    {
        using var client = this.GetHttpClient();

        var uri = await endpointProvider.GetEndpoint("people/update", cancellationToken, "MutableService");

        var response = await client.PatchAsJsonAsync(uri, person, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<PersonEntity> CreatePerson(PersonEntity person, CancellationToken cancellationToken)
    {
        using var client = this.GetHttpClient();

        var uri = await endpointProvider.GetEndpoint("people/create", cancellationToken, "MutableService");

        var response = await client.PutAsJsonAsync(uri, person, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<PersonEntity>(
            await response.Content.ReadAsStreamAsync(cancellationToken),
            optionsProvider.GetOptions(),
            cancellationToken) ?? throw new ArgumentException("Couldn't parse create person result!");
    }

    public async Task DeletePerson(uint id, CancellationToken cancellationToken)
    {
        using var client = this.GetHttpClient();

        var uri = await endpointProvider.GetEndpoint("people/delete", cancellationToken, "MutableService");

        ArgumentNullException.ThrowIfNull(uri);

        string formattedUriString = Smart.Format(uri!.ToString(), new { id });

        var response = await client.DeleteAsync(uri, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private HttpClient GetHttpClient()
    {
        return clientFactory.CreateClient("schleuben");
    }
}
