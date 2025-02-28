namespace Shared.Infrastructure.Configuration.Json;

/// <summary>
/// Represents the configuration settings for JSON serialization and deserialization.
/// </summary>
public sealed class JsonSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether JSON output should be formatted with indentation.
    /// </summary>
    /// <value>
    /// <c>true</c> if the JSON output should be indented; otherwise, <c>false</c>.
    /// </value>
    public bool WriteIndented { get; set; } = true;

    /// <summary>
    /// Gets or sets the type of reference handling to be used during JSON serialization and deserialization.
    /// </summary>
    /// <remarks>
    /// This property determines how object references are handled during JSON processing.
    /// The available options are defined in the <see cref="ReferenceHandlerType"/> enumeration:
    /// <list type="bullet">
    /// <item>
    ///     <description>
    ///         <see cref="ReferenceHandlerType.None"/>: No special reference handling is applied.
    ///     </description>
    /// </item>
    /// <item>
    ///     <description>
    ///         <see cref="ReferenceHandlerType.Preserve"/>:
    ///         References are preserved to maintain object references during serialization and deserialization.
    ///     </description>
    /// </item>
    /// <item>
    ///     <description>
    ///         <see cref="ReferenceHandlerType.Ignore"/>:
    ///             References are ignored, and objects are serialized without maintaining references.
    ///     </description>
    /// </item>
    /// </list>
    /// </remarks>
    public ReferenceHandlerType ReferenceHandler { get; set; } = ReferenceHandlerType.Preserve;
}
