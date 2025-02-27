namespace Shared.Infrastructure.Database.Entities;

/// <summary>
/// Represents an address entity.
/// </summary>
public sealed class AddressEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the address.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the street name of the address.
    /// </summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the house number of the address.
    /// </summary>
    public string HouseNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the city of the address.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the zip code of the address.
    /// </summary>
    public string ZipCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional information about the address.
    /// </summary>
    public string? AdditionalInfo { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the person associated with the address.
    /// </summary>
    public int PersonId { get; set; }

    /// <summary>
    /// Gets or sets the person associated with the address.
    /// </summary>
    public PersonEntity Person { get; set; } = null!;
}
