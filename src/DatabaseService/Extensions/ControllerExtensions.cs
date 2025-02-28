using Microsoft.AspNetCore.Mvc;

namespace DatabaseService.Extensions;

/// <summary>
/// Controller extensions
/// </summary>
public static partial class ControllerExtensions
{
    /// <summary>
    /// Handles an invalid ID.
    /// </summary>
    /// <param name="controller">Controller</param>
    /// <param name="logger">Logger</param>
    /// <param name="id">ID</param>
    /// <returns>Bad request if invalid or null if valid</returns>
    public static IActionResult? HandleId(this ControllerBase controller, ILogger logger, uint id)
    {
        const string reason = "Invalid id was provided!";

        if (id < 1)
        {
            LogInvalidId(logger, reason);

            return controller.BadRequest(reason);
        }

        return null;
    }

    [LoggerMessage(LogLevel.Warning, "{message}")]
    static partial void LogInvalidId(ILogger logger, string message);
}
