namespace Shared.Infrastructure.Database.Entities;

/// <summary>
/// Represents a person entity in the database.
/// </summary>
public sealed class PersonEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the person.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the first name of the person.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name of the person.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the birth date of the person.
    /// </summary>
    public DateOnly? BirthDate { get; set; }

    /// <summary>
    /// Gets or sets the telephone connections associated with the person.
    /// </summary>
    public ICollection<TelephoneConnectionEntity> TelephoneConnections { get; set; } = [];

    /// <summary>
    /// Gets or sets the addresses associated with the person.
    /// </summary>
    public ICollection<AddressEntity> Addresses { get; set; } = [];
}
