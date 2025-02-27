namespace Shared.Infrastructure.Database.Services;

/// <summary>
/// Represents a service for managing the database.
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// Migrates the database to the latest version.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task MigrateUp(CancellationToken cancellationToken);

    /// <summary>
    /// Migrates the database down to the specified version.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="version">Specific version</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task MigrateDown(CancellationToken cancellationToken, long version = 0);
}
