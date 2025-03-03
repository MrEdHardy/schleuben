using Shared.Infrastructure.Configuration;

namespace AppUI.Configuration;

/// <summary>
/// App settings
/// </summary>
public class AppSettings : IAddressSettings
{
    /// <inheritdoc/>
    public IDictionary<string, Uri> Addresses { get; set; } = new Dictionary<string, Uri>();
}
