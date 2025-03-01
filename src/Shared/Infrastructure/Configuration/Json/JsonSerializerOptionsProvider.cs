using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Infrastructure.Configuration.Json;

/// <summary>
/// Provides functionality to configure and supply JSON serializer options
/// for use within the application.
/// </summary>
public sealed class JsonSerializerOptionsProvider(IOptionsMonitor<JsonSettings> settings)
    : IOptionsProvider<JsonSerializerOptions>
{
    /// <inheritdoc/>
    public JsonSerializerOptions GetOptions()
    {
        var currentSettings = settings.CurrentValue;

        return new JsonSerializerOptions
        {
            WriteIndented = currentSettings.WriteIndented,
            ReferenceHandler = currentSettings.ReferenceHandler switch
            {
                ReferenceHandlerType.Preserve => ReferenceHandler.Preserve,
                ReferenceHandlerType.Ignore => ReferenceHandler.IgnoreCycles,
                _ => null,
            },
        };
    }
}
