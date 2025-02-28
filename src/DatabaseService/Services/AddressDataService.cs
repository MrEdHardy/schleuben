using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Database;
using Shared.Infrastructure.Database.Entities;

namespace DatabaseService.Services;

/// <summary>
/// Represents a service for managing address entities in the database.
/// </summary>
/// <param name="dbContext">Database context</param>
public sealed class AddressDataService(DatabaseContext dbContext) : IAddressDataService
{
    /// <inheritdoc/>
    public async Task<AddressEntity> CreateAddressAsync(AddressEntity address, CancellationToken cancellationToken)
    {
        dbContext.Add(address);

        await dbContext.SaveChangesAsync(cancellationToken);

        return address;
    }

    /// <inheritdoc/>
    public Task DeleteAddressAsync(uint id, CancellationToken cancellationToken)
    {
        var address = this.GetBaseAddressQuery()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (address is not null)
        {
            dbContext.Remove(address);
        }

        return dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task<AddressEntity?> GetAddressByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return this.GetBaseAddressQuery().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AddressEntity>> GetAddressesAsync(CancellationToken cancellationToken)
    {
        return await this.GetBaseAddressQuery().ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateAddressAsync(AddressEntity address, CancellationToken cancellationToken)
    {
        var oldRecord = await this.GetBaseAddressQuery()
            .FirstAsync(a => a.Id == address.Id, cancellationToken);

        dbContext.Update(address);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<AddressEntity> GetBaseAddressQuery()
    {
        return dbContext.Addresses
            .AsNoTracking()
            .Include(a => a.Person)
            .AsSingleQuery()
            .OrderBy(a => a.Id);
    }
}
