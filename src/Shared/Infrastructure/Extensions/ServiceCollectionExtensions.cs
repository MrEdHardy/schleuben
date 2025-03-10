﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Polly;
using Shared.Infrastructure.Configuration;
using Shared.Infrastructure.Configuration.Json;
using Shared.Infrastructure.Configuration.OpenApi;
using Shared.Infrastructure.Configuration.Resilience;
using System.Threading.RateLimiting;

namespace Shared.Infrastructure.Extensions;

/// <summary>
/// Contains general extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the JSON settings configuration to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration section containing JsonSettings</param>
    public static void AddJsonSettings(this IServiceCollection services, IConfigurationSection configuration)
    {
        services.Configure<JsonSettings>(configuration);

        services.AddSingleton<JsonSerializerOptionsProvider>();
    }

    /// <summary>
    /// Adds a resilient http factory
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <param name="identifier">Identifier</param>
    public static void AddResilientHttpClientFactory(
        this IServiceCollection services,
        IConfiguration configuration,
        string identifier)
    {
        services.Configure<ResilienceSettings>(configuration.GetSection("ResilienceSettings"));

        const string baseIdentifier = "schleuben-";

        services
            .AddHttpClient("schleuben", builder =>
            {
                builder.DefaultRequestHeaders.Add("data-service", baseIdentifier + identifier);
            })
            .AddSchleubenResilience(baseIdentifier + identifier);
    }

    /// <summary>
    /// Adds the Schleuben resilience configuration to the specified <see cref="IHttpClientBuilder"/>.
    /// </summary>
    /// <param name="builder">Http client builder</param>
    /// <param name="identifier">Identifier</param>
    /// <returns>Builder</returns>
    public static IHttpClientBuilder AddSchleubenResilience(this IHttpClientBuilder builder, string identifier)
    {
        builder.AddResilienceHandler($"pipeline-{identifier}", (handler, context) =>
        {
            context.EnableReloads<ResilienceSettings>();

            var settings = context.GetOptions<ResilienceSettings>();

            var loggerFactory = context.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(identifier);

            handler.AddSchleubenResilienceHandler(
                settings ?? ResilienceSettings.Default,
                logger);
        });

        return builder;
    }

    /// <summary>
    /// Register instance of <see cref="IAddressSettings"/>
    /// </summary>
    /// <typeparam name="TSettings"><see cref="IAddressSettings"/> type</typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="section">Configuration section containing the <see cref="IAddressSettings"/></param>
    public static void RegisterAddressSettings<TSettings>(
        this IServiceCollection services,
        IConfigurationSection section)
        where TSettings : class, IAddressSettings
    {
        services.Configure<TSettings>(section);

        services.AddSingleton<IOptionsMonitor<IAddressSettings>>(provider => provider
            .GetRequiredService<IOptionsMonitor<TSettings>>());
    }

    /// <summary>
    /// Initializes the initial endpoint discovery
    /// </summary>
    /// <param name="services">Service provider</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public static async Task ExecuteInitializeEndpointDiscovery(
        this IServiceProvider services,
        CancellationToken cancellationToken)
    {
        var endpointService = services.GetRequiredService<IEndpointProviderService>();

        await endpointService.InitializeEndpoints(cancellationToken);
    }

    private static void AddSchleubenResilienceHandler(
        this ResiliencePipelineBuilder<HttpResponseMessage> builder,
        ResilienceSettings settings,
        ILogger logger)
    {
        var tokenBucket = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions()
        {
            AutoReplenishment = true,

            // Enable a unlimited queue to circumvent potential RateLimiterRejectedExceptions
            QueueLimit = int.MaxValue,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,

            // For example a max of 10 tokens at the same time
            TokenLimit = (int)settings.RequestsPerSecond,

            // For every second replenish e.g. 10 tokens => 10 RPS
            ReplenishmentPeriod = TimeSpan.FromSeconds(1),
            TokensPerPeriod = (int)settings.RequestsPerSecond,
        });

        builder.AddRateLimiter(new HttpRateLimiterStrategyOptions
        {
            RateLimiter = async _ =>
            {
                var lease = await tokenBucket.AcquireAsync();

                return lease;
            },
            OnRejected = _ =>
            {
                logger.LogInformation("Request rejected due to rate-limit!");

                return ValueTask.CompletedTask;
            },
        });

        builder.AddConcurrencyLimiter((int)settings.RequestsPerSecond, int.MaxValue);

        builder.AddRetry(new HttpRetryStrategyOptions
        {
            BackoffType = DelayBackoffType.Exponential,
            MaxRetryAttempts = (int)settings.MaxRetryCount,
            ShouldHandle = args => HandleFailure(args.Outcome),
            OnRetry = args =>
            {
                logger.LogInformation(
                    "Retrying request due to {outcome}. Current retry {retryNumber}",
                    args.Outcome,
                    args.AttemptNumber + 1);

                return ValueTask.CompletedTask;
            },
            UseJitter = true,
            MaxDelay = settings.MaxDelay,
        });

        builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            BreakDuration = TimeSpan.FromSeconds(10),
            FailureRatio = settings.CircuitBreakerThreshold,
            SamplingDuration = settings.MaxRetryCount * TimeSpan.FromSeconds(30),
            ShouldHandle = args => HandleFailure(args.Outcome),
        });

        builder.AddTimeout(new HttpTimeoutStrategyOptions
        {
            Name = "Attempt timeout",
            Timeout = TimeSpan.FromSeconds(30),
            OnTimeout = args =>
            {
                logger.LogInformation("Request timeout of {seconds}s reached!", args.Timeout);

                return ValueTask.CompletedTask;
            },
        });
    }

    private static ValueTask<bool> HandleFailure(Outcome<HttpResponseMessage> outcome)
    {
        return ValueTask.FromResult(outcome switch
        {
            { Exception: HttpRequestException or HttpIOException or TimeoutException } => true,
            { Result.IsSuccessStatusCode: false } => true,
            _ => false,
        });
    }
}
