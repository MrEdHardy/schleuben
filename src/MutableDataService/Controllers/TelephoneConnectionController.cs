using Microsoft.AspNetCore.Mvc;
using MutableDataService.Services;
using Shared.Infrastructure.Database.Entities;
using System.Net;
using System.Net.Mime;

namespace MutableDataService.Controllers;

/// <summary>
/// Controller for managing telephone connections.
/// </summary>
/// <param name="dataService">Db service</param>
[ApiController]
[Route("telephone-connections")]
public sealed class TelephoneConnectionController(IMutableDatabaseService dataService) : ControllerBase
{
    /// <summary>
    /// Creates a new telephone connection.
    /// </summary>
    /// <param name="telephoneConnection">New telephone connection</param>
    /// <returns>Newly created telephone connection</returns>
    [HttpPut("Create")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(TelephoneConnectionEntity), (int)HttpStatusCode.OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<TelephoneConnectionEntity>> CreateTelephoneConnection(
        [FromBody] TelephoneConnectionEntity telephoneConnection)
    {
        var result = await dataService.CreateTelephoneConnectionAsync(
            telephoneConnection,
            this.HttpContext.RequestAborted);

        return this.Ok(result);
    }

    /// <summary>
    /// Deletes a telephone connection by its unique identifier.
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("Delete/{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> DeleteTelephoneConnection(uint id)
    {
        if (id < 1)
        {
            return this.BadRequest();
        }

        await dataService.DeleteTelephoneConnectionAsync(id, this.HttpContext.RequestAborted);
        return this.NoContent();
    }

    /// <summary>
    /// Updates a telephone connection.
    /// </summary>
    /// <param name="telephoneConnection">Connection to update</param>
    /// <returns>No content if successful</returns>
    [HttpPatch("Update")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> UpdateTelephoneConnection([FromBody] TelephoneConnectionEntity telephoneConnection)
    {
        await dataService.UpdateTelephoneConnectionAsync(telephoneConnection, this.HttpContext.RequestAborted);

        return this.NoContent();
    }
}
