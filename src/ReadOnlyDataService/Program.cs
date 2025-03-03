using Microsoft.Extensions.Options;
using ReadOnlyDataService.Configuration;
using ReadOnlyDataService.Services;
using Shared.Infrastructure.Configuration;
using Shared.Infrastructure.Configuration.Json;
using Shared.Infrastructure.Configuration.OpenApi;
using Shared.Infrastructure.Configuration.Resilience;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Middleware;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReadOnlyDataService;

/// <summary>
/// Represents the entry point of the application.
/// </summary>
public class Program
{
    /// <summary>
    /// The entry point of the application.
    /// </summary>
    /// <param name="args">Args</param>
    /// <returns>Task</returns>
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var tokenSource = new CancellationTokenSource();

        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            tokenSource.Cancel();
        };

        // Add services to the container.

        var jsonSettings = new JsonSettings();
        builder.Configuration.GetSection("JsonSettings").Bind(jsonSettings);

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.ReferenceHandler = jsonSettings.ReferenceHandler switch
                {
                    ReferenceHandlerType.Preserve => ReferenceHandler.Preserve,
                    ReferenceHandlerType.Ignore => ReferenceHandler.IgnoreCycles,
                    _ => null,
                };

                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.WriteIndented = jsonSettings.WriteIndented;
            });

        // Add logging
        builder.Services.AddLogging();

        // Add options and provider
        builder.Services.Configure<ReadOnlyDataServiceOptions>(
            builder.Configuration.GetSection("ReadOnlyDataService"));

        builder.Services.AddSingleton<IOptionsMonitor<IAddressSettings>>(provider => provider
            .GetRequiredService<IOptionsMonitor<ReadOnlyDataServiceOptions>>());

        // Add resiliency
        builder.Services.Configure<ResilienceSettings>(builder.Configuration.GetSection("ResilienceSettings"));

        const string identifier = "schleuben-readonly-database-service";

        builder.Services
            .AddHttpClient("schleuben", builder =>
            {
                builder.DefaultRequestHeaders.Add("data-service", identifier);
            })
            .AddSchleubenResilience(identifier);

        // Add database service
        builder.Services.AddScoped<IReadOnlyDatabaseService, ReadOnlyDatabaseService>();

        // Add endpoint provider
        builder.Services.AddSingleton<IEndpointProviderService, DefaultEndpointProviderService>();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.MapOpenApi();

        app.UseMiddleware<ErrorHandlerMiddleware>();
        app.UseRouting();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync(tokenSource.Token);
    }
}
