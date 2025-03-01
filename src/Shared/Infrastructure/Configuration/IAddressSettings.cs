namespace Shared.Infrastructure.Configuration;

/// <summary>
/// Represents the address settings.
/// </summary>
public interface IAddressSettings
{
    /// <summary>
    /// Gets or sets the addresses.
    /// </summary>
    IDictionary<string, Uri> Addresses { get; set; }
}
