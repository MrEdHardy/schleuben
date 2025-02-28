using Shared.Infrastructure.Database.Entities;

namespace DatabaseService.Services;

/// <summary>
/// Represents a service for managing telephone connections.
/// </summary>
public interface ITelephoneConnectionDataService
{
    /// <summary>
    /// Gets all telephone connections.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All telephone connections</returns>
    Task<IEnumerable<TelephoneConnectionEntity>> GetTelephoneConnectionsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets a telephone connection by ID.
    /// </summary>
    /// <param name="id">The ID of the telephone connection</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The telephone connection with the specified ID if found</returns>
    Task<TelephoneConnectionEntity?> GetTelephoneConnectionByIdAsync(uint id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new telephone connection.
    /// </summary>
    /// <param name="telephoneConnection">The telephone connection to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created telephone connection</returns>
    Task<TelephoneConnectionEntity> CreateTelephoneConnectionAsync(
        TelephoneConnectionEntity telephoneConnection,
        CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing telephone connection.
    /// </summary>
    /// <param name="telephoneConnection">The telephone connection to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateTelephoneConnectionAsync(
        TelephoneConnectionEntity telephoneConnection,
        CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a telephone connection by ID.
    /// </summary>
    /// <param name="id">The ID of the telephone connection to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteTelephoneConnectionAsync(uint id, CancellationToken cancellationToken);
}
