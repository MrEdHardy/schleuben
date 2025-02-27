using FluentMigrator.Runner;

namespace Shared.Infrastructure.Database.Services;

/// <summary>
/// Represents a service for managing the database.
/// </summary>
/// <param name="provider">Service provider</param>
public sealed partial class FluentMigratorService(IServiceProvider provider) : IDatabaseService
{
    /// <inheritdoc/>
    public Task MigrateDown(CancellationToken cancellationToken, long version = 0)
    {
        using var scope = provider.CreateScope();
        var (logger, runner) = GetLoggerAndRunner(scope);

        logger.LogInformation("Migrating the database down to version {Version}...", version);

        try
        {
            runner.MigrateDown(version);
        }
        catch (Exception e)
        {
            LogError(logger, e);

            throw;
        }

        logger.LogInformation("The database has been migrated down to version {Version}.", version);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task MigrateUp(CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var (logger, runner) = GetLoggerAndRunner(scope);

        logger.LogInformation("Migrating the database to the latest version...");

        runner.ListMigrations();

        try
        {
            runner.MigrateUp();
        }
        catch (Exception e)
        {
            LogError(logger, e);

            runner.RollbackToVersion(0);

            throw;
        }

        logger.LogInformation("The database has been migrated to the latest version.");

        return Task.CompletedTask;
    }

    private static (ILogger logger, IMigrationRunner runner) GetLoggerAndRunner(IServiceScope scope)
    {
        return (scope.ServiceProvider.GetRequiredService<ILogger<FluentMigratorService>>(),
            scope.ServiceProvider.GetRequiredService<IMigrationRunner>());
    }

    [LoggerMessage(LogLevel.Error, "An error occurred while migrating the database.")]
    static partial void LogError(ILogger logger, Exception e);
}
