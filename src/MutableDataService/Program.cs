
using MutableDataService.Configuration;
using MutableDataService.Services;
using Shared.Infrastructure.Configuration.Json;
using Shared.Infrastructure.Configuration.Resilience;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Middleware;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MutableDataService;

/// <summary>
/// Represents the entry point of the application.
/// </summary>
public class Program
{
    /// <summary>
    /// The entry point of the application.
    /// </summary>
    /// <param name="args">Args</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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

        // Add options
        builder.Services.Configure<MutableDataServiceOptions>(
            builder.Configuration.GetSection("MutableDataService"));

        builder.Services.Configure<ResilienceSettings>(
            builder.Configuration.GetSection("ResilienceSettings"));

        // Add endpoint provider
        builder.Services.AddSingleton<EndpointProviderService>();

        // Add http client and resilience
        const string identifier = "schleuben-mutable-database-service";

        builder.Services
            .AddHttpClient("schleuben", builder =>
            {
                builder.DefaultRequestHeaders.Add("data-service", identifier);
            })
            .AddSchleubenResilience(identifier);

        // Add mutable data service
        builder.Services.AddScoped<IMutableDatabaseService, MutableDatabaseService>();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Add middleware
        app.UseMiddleware<ErrorHandlerMiddleware>();
        app.UseRouting();

        // Configure the HTTP request pipeline.
        app.MapOpenApi();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
