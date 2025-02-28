using DatabaseService.Extensions;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Database;
using Shared.Infrastructure.Database.Entities;

namespace DatabaseService.Services;

/// <summary>
/// Represents a service for managing telephone connection entities in the database.
/// </summary>
/// <param name="dbContext">Database context</param>
public sealed class TelephoneConnectionDataService(DatabaseContext dbContext) : ITelephoneConnectionDataService
{
    /// <inheritdoc/>
    public Task<TelephoneConnectionEntity> CreateTelephoneConnectionAsync(
        TelephoneConnectionEntity telephoneConnection,
        CancellationToken cancellationToken)
    {
        return dbContext.CreateEntity(telephoneConnection, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteTelephoneConnectionAsync(uint id, CancellationToken cancellationToken)
    {
        var entity = await this.GetBaseTelephoneConnectionQuery()
            .FirstOrDefaultAsync(tc => tc.Id == id, cancellationToken);

        await dbContext.DeleteEntity(entity, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<TelephoneConnectionEntity?> GetTelephoneConnectionByIdAsync(
        uint id,
        CancellationToken cancellationToken)
    {
        return this.GetBaseTelephoneConnectionQuery()
            .FirstOrDefaultAsync(tc => tc.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TelephoneConnectionEntity>> GetTelephoneConnectionsAsync(
        CancellationToken cancellationToken)
    {
        return await this.GetBaseTelephoneConnectionQuery()
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateTelephoneConnectionAsync(
        TelephoneConnectionEntity telephoneConnection,
        CancellationToken cancellationToken)
    {
        var oldConnection = await this.GetBaseTelephoneConnectionQuery()
            .FirstAsync(tc => tc.Id == telephoneConnection.Id, cancellationToken);

        await dbContext.UpdateEntity(telephoneConnection, cancellationToken);
    }

    private IQueryable<TelephoneConnectionEntity> GetBaseTelephoneConnectionQuery()
    {
        return dbContext.TelephoneConnections
            .AsNoTracking()
            .Include(tc => tc.Person)
            .AsSingleQuery()
            .OrderBy(tc => tc.Id);
    }
}
