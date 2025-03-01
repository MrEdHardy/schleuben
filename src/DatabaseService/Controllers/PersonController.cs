using DatabaseService.Extensions;
using DatabaseService.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Infrastructure.Database.Entities;
using System.Net;
using System.Net.Mime;

namespace DatabaseService.Controllers;

/// <summary>
/// Person controller
/// </summary>
/// <param name="logger"></param>
/// <param name="dataService"></param>
[ApiController]
[Route("people")]
public class PersonController(
    ILogger<PersonController> logger,
    IPersonDataService dataService) : ControllerBase
{
    /// <summary>
    /// Retrieves all persons from the database.
    /// </summary>
    /// <remarks>
    /// This method fetches all records from the <see cref="PersonEntity"/> table in the database,
    /// serializes them into a JSON string, and returns the result.
    /// </remarks>
    /// <returns>
    /// A JSON string representing the collection of all persons in the database.
    /// </returns>
    /// <response code="200">Returns the JSON string of all persons.</response>
    /// <response code="500">If an error occurs while accessing the database.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PersonEntity>), (int)HttpStatusCode.OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<PersonEntity>>> GetAllPersons()
    {
        return this.Ok(await dataService.GetPeopleAsync(this.HttpContext.RequestAborted));
    }

    /// <summary>
    /// Retrieves a person by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the person to retrieve.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> containing the serialized person data if found,
    /// or an appropriate HTTP response indicating the result of the operation.
    /// </returns>
    /// <response code="200">Returns the JSON string of the person with the specified ID.</response>
    /// <response code="400">If the provided ID is invalid (less than 1).</response>
    /// <response code="404">If no person with the specified ID is found.</response>
    /// <response code="500">If an error occurs while accessing the database.</response>
    [HttpGet("GetPersonById/{id}")]
    [ProducesResponseType(typeof(PersonEntity), (int)HttpStatusCode.OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<PersonEntity>> GetPersonById(uint id)
    {
        var idHandleResult = this.HandleId(logger, id);

        if (idHandleResult is not null)
        {
            return idHandleResult;
        }

        var result = await dataService.GetPersonByIdAsync(id, this.HttpContext.RequestAborted);

        return result is not null
            ? this.Ok(result)
            : this.NotFound($"No person with given id {id} was found!");
    }

    /// <summary>
    /// Updates a person in the database.
    /// </summary>
    /// <param name="person">Person to be updated</param>
    /// <returns>No content if it succeeds</returns>
    [HttpPatch("UpdatePerson")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> UpdatePerson([FromBody] PersonEntity person)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        await dataService.UpdatePersonAsync(person, this.HttpContext.RequestAborted);

        return this.NoContent();
    }

    /// <summary>
    /// Create a person in the database.
    /// </summary>
    /// <param name="person">Person to create</param>
    /// <returns>The created person</returns>
    [HttpPut("CreatePerson")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(PersonEntity), (int)HttpStatusCode.Created, MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<PersonEntity>> CreatePerson([FromBody] PersonEntity person)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var result = await dataService.CreatePersonAsync(person, this.HttpContext.RequestAborted);

        return this.CreatedAtAction(nameof(GetPersonById), new { result.Id }, result);
    }

    /// <summary>
    /// Delete a person from the database.
    /// </summary>
    /// <param name="id">Person id to delete</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("DeletePerson/{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> DeletePerson(uint id)
    {
        var idHandleResult = this.HandleId(logger, id);

        if (idHandleResult is not null)
        {
            return idHandleResult;
        }

        try
        {
            await dataService.DeletePersonAsync(id, this.HttpContext.RequestAborted);

        }
        catch (InvalidOperationException e)
        {
            const string message = "Cannot delete a person with addresses or telephone connections.";

            logger.LogError(e, message + " Id: {id}", id);

            return this.BadRequest(message);
        }

        return this.NoContent();
    }
}
