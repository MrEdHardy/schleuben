
using MutableDataService.Configuration;
using Shared.Infrastructure.Configuration.Json;
using Shared.Infrastructure.Configuration.Resilience;
using Shared.Infrastructure.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MutableDataService;

public class Program
{
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

        // Add http client and resilience
        const string identifier = "schleuben-mutable-database-service";

        builder.Services
            .AddHttpClient("schleuben", builder =>
            {
                builder.DefaultRequestHeaders.Add("data-service", identifier);
            })
            .AddSchleubenResilience(identifier);

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.MapOpenApi();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
