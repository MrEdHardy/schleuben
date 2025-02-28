using Shared.Infrastructure.Database.Entities;

namespace DatabaseService.Services;

/// <summary>
/// Interface for address data service operations.
/// </summary>
public interface IAddressDataService
{
    /// <summary>
    /// Creates a new address asynchronously.
    /// </summary>
    /// <param name="address">The address entity to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created address entity.</returns>
    Task<AddressEntity> CreateAddressAsync(AddressEntity address, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an address by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the address to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAddressAsync(uint id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all addresses asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of address entities.</returns>
    Task<IEnumerable<AddressEntity>> GetAddressesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves an address by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the address to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The address entity if found; otherwise, null.</returns>
    Task<AddressEntity?> GetAddressByIdAsync(uint id, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing address asynchronously.
    /// </summary>
    /// <param name="address">The address entity to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAddressAsync(AddressEntity address, CancellationToken cancellationToken);
}
