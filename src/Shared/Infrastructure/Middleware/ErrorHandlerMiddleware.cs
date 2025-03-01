using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace Shared.Infrastructure.Middleware;

/// <summary>
/// Middleware for handling unhandled exceptions.
/// </summary>
/// <param name="next">Next pipeline step</param>
/// <param name="logger">Logger</param>
public sealed partial class ErrorHandlerMiddleware(
    RequestDelegate next,
    ILogger<ErrorHandlerMiddleware> logger)
{
    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">Http context</param>
    /// <returns>Task</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception e)
        {
            LogError(logger, e);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                error = "An error occurred while processing the request.",
            }));
        }
    }

    [LoggerMessage(LogLevel.Error, "An unhandled exception has occurred.")]
    private static partial void LogError(ILogger logger, Exception e);
}
