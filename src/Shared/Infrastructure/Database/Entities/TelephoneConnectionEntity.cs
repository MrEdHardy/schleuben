using System.Text.Json.Serialization;

namespace Shared.Infrastructure.Database.Entities;

/// <summary>
/// Represents a telephone connection entity.
/// </summary>
public sealed class TelephoneConnectionEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the telephone connection.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the phone number associated with the telephone connection.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the person associated with the telephone connection.
    /// </summary>
    public int PersonId { get; set; }

    /// <summary>
    /// Gets or sets the person associated with the telephone connection.
    /// </summary>
    [JsonIgnore]
    public PersonEntity? Person { get; set; }
}
