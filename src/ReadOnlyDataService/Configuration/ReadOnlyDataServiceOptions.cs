using Shared.Infrastructure.Configuration;

namespace ReadOnlyDataService.Configuration;

/// <summary>
/// Represents the options for the read-only data service.
/// </summary>
public sealed class ReadOnlyDataServiceOptions : IAddressSettings
{
    /// <inheritdoc/>
    public IDictionary<string, Uri> Addresses { get; set; } = new Dictionary<string, Uri>();
}
