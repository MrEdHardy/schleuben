using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Database;
using Shared.Infrastructure.Database.Entities;

namespace DatabaseService.Services;

/// <summary>
/// Represents a service for managing person entities in the database.
/// </summary>
/// <param name="dbContext">Database context</param>
public sealed class PersonDataService(DatabaseContext dbContext) : IPersonDataService
{
    /// <inheritdoc/>
    public async Task<PersonEntity> CreatePersonAsync(PersonEntity person, CancellationToken cancellationToken)
    {
        dbContext.Add(person);

        await dbContext.SaveChangesAsync(cancellationToken);

        return person;
    }

    /// <inheritdoc/>
    public async Task DeletePersonAsync(uint id, CancellationToken cancellationToken)
    {
        var entity = await this.GetBasePersonQuery()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (entity is not null)
        {
            dbContext.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PersonEntity>> GetPeopleAsync(CancellationToken cancellationToken)
    {
        return await this.GetBasePersonQuery()
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task<PersonEntity?> GetPersonByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return this.GetBasePersonQuery()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdatePersonAsync(PersonEntity person, CancellationToken cancellationToken)
    {
        var oldRecord = await this.GetBasePersonQuery()
            .FirstAsync(p => p.Id == person.Id, cancellationToken);

        dbContext.Update(person);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<PersonEntity> GetBasePersonQuery()
    {
        return dbContext.People
            .AsNoTracking()
            .Include(p => p.Addresses)
            .Include(p => p.TelephoneConnections)
            .AsSplitQuery()
            .OrderBy(p => p.Id);
    }
}
