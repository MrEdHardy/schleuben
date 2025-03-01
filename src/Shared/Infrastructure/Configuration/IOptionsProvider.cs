namespace Shared.Infrastructure.Configuration;

/// <summary>
/// Represents an options provider.
/// </summary>
/// <typeparam name="TOptions">Options type</typeparam>
public interface IOptionsProvider<out TOptions>
    where TOptions : notnull
{
    /// <summary>
    /// Gets the options.
    /// </summary>
    /// <returns>A <typeparamref name="TOptions"/> instance</returns>
    TOptions GetOptions();
}
