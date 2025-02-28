using Shared.Infrastructure.Database;

namespace DatabaseService.Extensions;

/// <summary>
/// Extension methods for <see cref="DatabaseContext"/>.
/// </summary>
public static class DatabaseContextExtensions
{
    /// <summary>
    /// Creates an entity in the database.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="dbContext">Database</param>
    /// <param name="entity">Entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Newly created entity</returns>
    public static async Task<TEntity> CreateEntity<TEntity>(
        this DatabaseContext dbContext,
        TEntity entity,
        CancellationToken cancellationToken)
        where TEntity : class
    {
        dbContext.Add(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    /// <summary>
    /// Updates an entity in the database.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="dbContext">Database</param>
    /// <param name="entity">Entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public static async Task UpdateEntity<TEntity>(
        this DatabaseContext dbContext,
        TEntity? entity,
        CancellationToken cancellationToken)
        where TEntity : class
    {
        if (entity is null)
        {
            return;
        }

        dbContext.Update(entity);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes an entity from the database.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="dbContext">Database</param>
    /// <param name="entity">Entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public static async Task DeleteEntity<TEntity>(
        this DatabaseContext dbContext,
        TEntity? entity,
        CancellationToken cancellationToken)
        where TEntity : class
    {
        if (entity is null)
        {
            return;
        }

        dbContext.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
