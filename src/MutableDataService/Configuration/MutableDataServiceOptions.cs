using Shared.Infrastructure.Configuration;

namespace MutableDataService.Configuration;

/// <summary>
/// Represents the options for the mutable data service.
/// </summary>
public sealed class MutableDataServiceOptions : IAddressSettings
{
    /// <inheritdoc/>
    public IDictionary<string, Uri> Addresses { get; set; } = new Dictionary<string, Uri>();
}
