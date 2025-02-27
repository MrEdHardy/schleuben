
using FluentMigrator.Runner;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Database;
using Shared.Infrastructure.Database.Migrations;
using Shared.Infrastructure.Database.Services;
using System.IO.Abstractions;

namespace schleuben;

/// <summary>
/// Represents the entry point of the application.
/// </summary>
public class Program
{
    /// <summary>
    /// The entry point of the application.
    /// </summary>
    /// <param name="args"></param>
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        // Custom services
        builder.Services.AddLogging();

        var fs = new FileSystem();

        const string fallbackConnectionString = "Data Source=Db/database.db; foreign keys=true";
        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? fallbackConnectionString;

        var sqliteConnectionString = new SqliteConnectionStringBuilder(connectionString);

        var fileInfoDbFile = fs.FileInfo.New(sqliteConnectionString.DataSource);

        if (!fileInfoDbFile.Directory?.Exists ?? false)
        {
            fileInfoDbFile.Directory?.Create();
        }

        // Add db context
        builder.Services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseSqlite(sqliteConnectionString.ToString());
        });

        // Add fluent migrator
        builder.Services.AddFluentMigratorCore()
            .ConfigureRunner(runnerBuilder =>
            {
                runnerBuilder.AddSQLite()
                    .WithGlobalConnectionString(sqliteConnectionString.ToString())
                    .ScanIn(typeof(InitialMigration).Assembly).For.Migrations();
            });

        // Add database service
        builder.Services.AddSingleton<IDatabaseService, FluentMigratorService>();

        // Add environment and file system
        var systemEnvironment = SystemEnvironmentProvider.Instance;

        builder.Services.AddSingleton(systemEnvironment);
        builder.Services.AddSingleton<IFileSystem>(fs);

        // Cancellation token source
        var cts = new CancellationTokenSource();

        Console.CancelKeyPress += async (sender, eventArgs) =>
        {
            eventArgs.Cancel = true;
            await cts.CancelAsync();
        };

        builder.Services.AddSingleton(cts);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        var dbService = app.Services.GetRequiredService<IDatabaseService>();

        await dbService.MigrateUp(cts.Token);

        app.Run();
    }
}
