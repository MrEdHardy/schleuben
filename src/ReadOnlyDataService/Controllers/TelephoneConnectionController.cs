using Microsoft.AspNetCore.Mvc;
using ReadOnlyDataService.Services;
using Shared.Infrastructure.Database.Entities;
using System.Net;
using System.Net.Mime;

namespace ReadOnlyDataService.Controllers;

/// <summary>
/// Controller for managing telephone connections.
/// </summary>
[ApiController]
[Route("telephone-connections")]
public sealed class TelephoneConnectionController(IReadOnlyDatabaseService dbService) : ControllerBase
{
    /// <summary>
    /// Retrieves all telephone connections.
    /// </summary>
    /// <returns>Collection of connections</returns>
    [HttpGet]
    [ProducesResponseType(typeof(
        IEnumerable<TelephoneConnectionEntity>), (int)HttpStatusCode.OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<TelephoneConnectionEntity>>> GetAllTelephoneConnections()
    {
        return this.Ok(await dbService.GetTelephoneConnectionsAsync(this.HttpContext.RequestAborted));
    }

    /// <summary>
    /// Retrieves a telephone connection by its unique identifier.
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <returns>Connection if found</returns>
    [HttpGet("getbyid/{id}")]
    [ProducesResponseType(typeof(TelephoneConnectionEntity), (int)HttpStatusCode.OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<TelephoneConnectionEntity>> GetTelephoneConnectionById(uint id)
    {
        if (id < 1)
        {
            return this.BadRequest();
        }

        var result = await dbService.GetTelephoneConnectionByIdAsync(id, this.HttpContext.RequestAborted);

        return result is null ? this.NotFound() : this.Ok(result);
    }
}
