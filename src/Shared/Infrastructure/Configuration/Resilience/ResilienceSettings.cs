namespace Shared.Infrastructure.Configuration.Resilience;

/// <summary>
/// Represents the resilience settings.
/// </summary>
public sealed class ResilienceSettings
{
    /// <summary>
    /// Gets the default resilience settings.
    /// </summary>
    public static ResilienceSettings Default => new()
    {
        MaxRetryCount = 3,
        MaxDelay = TimeSpan.FromMinutes(5),
        CircuitBreakerThreshold = 0.2d,
        RequestsPerSecond = 10,
    };

    /// <summary>
    /// Gets or sets the maximum retry count.
    /// </summary>
    public uint MaxRetryCount { get; set; }

    /// <summary>
    /// Gets or sets the maximum delay between retries.
    /// </summary>
    public TimeSpan MaxDelay { get; set; }

    /// <summary>
    /// Gets or sets the circuit breaker threshold.
    /// </summary>
    public double CircuitBreakerThreshold { get; set; }

    /// <summary>
    /// Gets or sets the circuit breaker duration.
    /// </summary>
    public uint RequestsPerSecond { get; set; }
}
