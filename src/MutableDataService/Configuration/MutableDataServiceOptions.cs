using Shared.Infrastructure.Configuration;

namespace MutableDataService.Configuration;

/// <summary>
/// Represents the options for the mutable data service.
/// </summary>
public sealed class MutableDataServiceOptions : IAddressSettings
{
    /// <inheritdoc/>
    public IDictionary<string, Uri> Addresses { get; set; } = new Dictionary<string, Uri>();

    /// <summary>
    /// Gets or sets the base URL for the mutable data service.
    /// </summary>
    public string OpenApiPath { get; set; } = "/openapi/v1.json";

    /// <summary>
    /// Gets or sets the test flag.
    /// </summary>
    public bool Test { get; set; }
}
