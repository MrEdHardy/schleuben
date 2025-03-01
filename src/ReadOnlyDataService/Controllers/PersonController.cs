using Microsoft.AspNetCore.Mvc;
using ReadOnlyDataService.Services;
using Shared.Infrastructure.Database.Entities;
using System.Net;
using System.Net.Mime;

namespace ReadOnlyDataService.Controllers;

/// <summary>
/// Person controller
/// </summary>
[ApiController]
[Route("people")]
public sealed class PersonController(
    IReadOnlyDatabaseService dbService) : ControllerBase
{
    /// <summary>
    /// Retrieves all people from the database.
    /// </summary>
    /// <returns>Collection of people</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PersonEntity>), (int)HttpStatusCode.OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<PersonEntity>>> GetAllPeople()
    {
        return this.Ok(await dbService.GetPeopleAsync(this.HttpContext.RequestAborted));
    }

    /// <summary>
    /// Retrieves a person by their unique identifier.
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <returns>Person if found</returns>
    [HttpGet("getbyid/{id}")]
    [ProducesResponseType(typeof(PersonEntity), (int)HttpStatusCode.OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<PersonEntity>> GetPersonById(uint id)
    {
        if (id < 1)
        {
            return this.BadRequest("Invalid id!");
        }

        var result = await dbService.GetPersonByIdAsync(id, this.HttpContext.RequestAborted);

        return result is null
            ? this.NotFound()
            : this.Ok(result);
    }
}
