﻿using DatabaseService.Extensions;
using DatabaseService.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Infrastructure.Database.Entities;
using System.Net;
using System.Net.Mime;

namespace DatabaseService.Controllers;

/// <summary>
/// Controller for managing telephone connections.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TelephoneConnectionController"/> class.
/// </remarks>
/// <param name="dataService">The data service for telephone connections.</param>
/// <param name="logger">The logger instance.</param>
[ApiController]
[Route("telephone-connections")]
public class TelephoneConnectionController(
    ITelephoneConnectionDataService dataService,
    ILogger<TelephoneConnectionController> logger) : ControllerBase
{

    /// <summary>
    /// Retrieves all telephone connections.
    /// </summary>
    /// <returns>Collection of all connections</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TelephoneConnectionEntity>), (int)HttpStatusCode.OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<TelephoneConnectionEntity>>> GetAllTelephoneConnections()
    {
        return this.Ok(await dataService.GetTelephoneConnectionsAsync(this.HttpContext.RequestAborted));
    }

    /// <summary>
    /// Retrieves a telephone connection by its unique identifier.
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <returns>Found connection or bad request or not found</returns>
    [HttpGet("GetTelephoneConnectionById/{id}")]
    [ProducesResponseType(typeof(TelephoneConnectionEntity), (int)HttpStatusCode.OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<TelephoneConnectionEntity>> GetTelephoneConnectionById(uint id)
    {
        var idHandleResult = this.HandleId(logger, id);

        if (idHandleResult is not null)
        {
            return idHandleResult;
        }

        var result = await dataService.GetTelephoneConnectionByIdAsync(id, this.HttpContext.RequestAborted);

        return result is null ? this.NotFound() : this.Ok(result);
    }

    /// <summary>
    /// Updates an existing telephone connection.
    /// </summary>
    /// <param name="telephoneConnection">Connection to update</param>
    /// <returns>No content</returns>
    [HttpPatch("UpdateTelephoneConnection")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> UpdateTelephoneConnection(
        [FromBody] TelephoneConnectionEntity telephoneConnection)
    {
        await dataService.UpdateTelephoneConnectionAsync(telephoneConnection, this.HttpContext.RequestAborted);
        return this.NoContent();
    }

    /// <summary>
    /// Creates a new telephone connection.
    /// </summary>
    /// <param name="telephoneConnection">Connection to create</param>
    /// <returns>Newly created connection</returns>
    [HttpPut("CreateTelephoneConnection")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(
        typeof(TelephoneConnectionEntity), (int)HttpStatusCode.Created, MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<TelephoneConnectionEntity>> CreateTelephoneConnection(
        [FromBody] TelephoneConnectionEntity telephoneConnection)
    {
        var result = await dataService.CreateTelephoneConnectionAsync(
            telephoneConnection,
            this.HttpContext.RequestAborted);

        return this.CreatedAtAction(nameof(GetTelephoneConnectionById), new { result.Id }, result);
    }

    /// <summary>
    /// Deletes a telephone connection by its ID.
    /// </summary>
    /// <param name="id">The ID of the telephone connection to delete.</param>
    /// <returns>No content if the deletion was successful, otherwise a bad request or not found result.</returns>
    [HttpDelete("DeleteTelephoneConnection/{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> DeleteTelephoneConnection(uint id)
    {
        var idHandleResult = this.HandleId(logger, id);

        if (idHandleResult is not null)
        {
            return idHandleResult;
        }

        await dataService.DeleteTelephoneConnectionAsync(id, this.HttpContext.RequestAborted);

        return this.NoContent();
    }
}
