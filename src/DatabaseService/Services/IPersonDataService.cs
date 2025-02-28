using Shared.Infrastructure.Database.Entities;

namespace DatabaseService.Services;

/// <summary>
/// Represents a service for managing person data.
/// </summary>
public interface IPersonDataService
{
    /// <summary>
    /// Gets all people.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All people</returns>
    Task<IEnumerable<PersonEntity>> GetPeopleAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets a person by ID.
    /// </summary>
    /// <param name="id">The ID of the person</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The person with the specified ID if found</returns>
    Task<PersonEntity?> GetPersonByIdAsync(uint id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new person.
    /// </summary>
    /// <param name="person">The person to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created person</returns>
    Task<PersonEntity> CreatePersonAsync(PersonEntity person, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing person.
    /// </summary>
    /// <param name="person">The person to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdatePersonAsync(PersonEntity person, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a person by ID.
    /// </summary>
    /// <param name="id">The ID of the person to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeletePersonAsync(uint id, CancellationToken cancellationToken);
}
