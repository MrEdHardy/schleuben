using Shared.Infrastructure.Database.Entities;

namespace MutableDataService.Services;

/// <summary>
/// Interface for mutable data service operations.
/// </summary>
public interface IMutableDatabaseService
{
    /// <summary>
    /// Creates a new person asynchronously.
    /// </summary>
    /// <param name="person">The person entity to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created person entity.</returns>
    Task<PersonEntity> CreatePersonAsync(PersonEntity person, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a person asynchronously.
    /// </summary>
    /// <param name="id">The ID of the person to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeletePersonAsync(uint id, CancellationToken cancellationToken);

    /// <summary>
    /// Updates a person asynchronously.
    /// </summary>
    /// <param name="person">The person entity to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdatePersonAsync(PersonEntity person, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new address asynchronously.
    /// </summary>
    /// <param name="address">The address entity to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created address entity.</returns>
    Task<AddressEntity> CreateAddressAsync(AddressEntity address, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an address asynchronously.
    /// </summary>
    /// <param name="id">The ID of the address to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAddressAsync(uint id, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an address asynchronously.
    /// </summary>
    /// <param name="address">The address entity to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAddressAsync(AddressEntity address, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new telephone connection asynchronously.
    /// </summary>
    /// <param name="telephoneConnection">The telephone connection entity to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created telephone connection entity.</returns>
    Task<TelephoneConnectionEntity> CreateTelephoneConnectionAsync(
        TelephoneConnectionEntity telephoneConnection,
        CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a telephone connection asynchronously.
    /// </summary>
    /// <param name="id">The ID of the telephone connection to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteTelephoneConnectionAsync(uint id, CancellationToken cancellationToken);

    /// <summary>
    /// Updates a telephone connection asynchronously.
    /// </summary>
    /// <param name="telephoneConnection">The telephone connection entity to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateTelephoneConnectionAsync(
        TelephoneConnectionEntity telephoneConnection,
        CancellationToken cancellationToken);
}
