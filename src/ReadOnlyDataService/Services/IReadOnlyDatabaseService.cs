using Shared.Infrastructure.Database.Entities;

namespace ReadOnlyDataService.Services;

/// <summary>
/// Interface for read-only database service.
/// </summary>
public interface IReadOnlyDatabaseService
{
    /// <summary>
    /// Gets a list of people asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of people.</returns>
    Task<IEnumerable<PersonEntity>> GetPeopleAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets a list of addresses asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of addresses.</returns>
    Task<IEnumerable<AddressEntity>> GetAddressesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets a list of telephone connections asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of telephone connections.</returns>
    Task<IEnumerable<TelephoneConnectionEntity>> GetTelephoneConnectionsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets a person by ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the person.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the person entity.</returns>
    Task<PersonEntity?> GetPersonByIdAsync(uint id, CancellationToken cancellationToken);

    /// <summary>
    /// Gets an address by ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the address entity.</returns>
    Task<AddressEntity?> GetAddressByIdAsync(uint id, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a telephone connection by ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the telephone connection.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the telephone connection entity.</returns>
    Task<TelephoneConnectionEntity?> GetTelephoneConnectionByIdAsync(uint id, CancellationToken cancellationToken);
}
