using Microsoft.AspNetCore.Mvc;
using MutableDataService.Services;
using Shared.Infrastructure.Database.Entities;
using System.Net;
using System.Net.Mime;

namespace MutableDataService.Controllers;

/// <summary>
/// Controller for mutating people.
/// </summary>
/// <param name="dataService">Database service</param>
[ApiController]
[Route("people")]
public class PersonController(IMutableDatabaseService dataService) : ControllerBase
{
    /// <summary>
    /// Creates a new person.
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    [HttpPut("Create")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(PersonEntity), (int)HttpStatusCode.OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> CreatePerson([FromBody] PersonEntity person)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var result = await dataService.CreatePersonAsync(person, this.HttpContext.RequestAborted);

        return this.Ok(result);
    }

    /// <summary>
    /// Retrieves a person by its unique identifier.
    /// </summary>
    /// <param name="person">Identifier</param>
    /// <returns>No content if successful</returns>
    [HttpPatch("Update")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
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
    /// Deletes a person by its unique identifier.
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("Delete/{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> DeletePerson(uint id)
    {
        if (id < 1)
        {
            return this.BadRequest();
        }

        await dataService.DeletePersonAsync(id, this.HttpContext.RequestAborted);

        return this.NoContent();
    }
}
