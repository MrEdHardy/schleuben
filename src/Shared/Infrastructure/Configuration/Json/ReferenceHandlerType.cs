namespace Shared.Infrastructure.Configuration.Json;

/// <summary>
/// Specifies the type of reference handling to be used during JSON serialization and deserialization.
/// </summary>
/// <summary>
/// No special reference handling is applied.
/// </summary>
/// <summary>
/// References are preserved to maintain object references during serialization and deserialization.
/// </summary>
/// <summary>
/// References are ignored, and objects are serialized without maintaining references.
/// </summary>
public enum ReferenceHandlerType : sbyte
{
    /// <summary>
    /// Represents the default reference handling behavior where no special handling is applied.
    /// </summary>
    None = 0,

    /// <summary>
    /// Specifies that object references should be preserved during serialization and deserialization.
    /// </summary>
    Preserve = 1,

    /// <summary>
    /// Specifies that reference handling should ignore circular references during serialization.
    /// </summary>
    Ignore = 2,
}
