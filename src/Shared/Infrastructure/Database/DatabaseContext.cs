using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Database.Entities;

namespace Shared.Infrastructure.Database;

/// <summary>
/// Represents the database context.
/// </summary>
/// <param name="dbContextOptions">Database context options</param>
public sealed class DatabaseContext(DbContextOptions<DatabaseContext> dbContextOptions) : DbContext(dbContextOptions)
{
    /// <summary>
    /// Get the people in the database.
    /// </summary>
    public DbSet<PersonEntity> People => this.Set<PersonEntity>();

    /// <summary>
    /// Gets the addresses in the database.
    /// </summary>
    public DbSet<AddressEntity> Addresses => this.Set<AddressEntity>();

    /// <summary>
    /// Gets the telephone connections in the database.
    /// </summary>
    public DbSet<TelephoneConnectionEntity> TelephoneConnections => this.Set<TelephoneConnectionEntity>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
