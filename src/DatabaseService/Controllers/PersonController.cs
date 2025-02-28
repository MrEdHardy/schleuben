using DatabaseService.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Infrastructure.Database.Entities;

namespace DatabaseService.Controllers;

/// <summary>
/// Person controller
/// </summary>
/// <param name="logger"></param>
/// <param name="dataService"></param>
[ApiController]
[Route("persons")]
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
    [HttpGet("GetAllPersons")]
    public async Task<IActionResult> GetAllPersons()
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
    public async Task<IActionResult> GetPersonById(uint id)
    {
        if (id < 1)
        {
            const string reason = "Invalid id was provided!";

            logger.LogWarning(reason);

            return this.BadRequest(reason);
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
    public async Task<IActionResult> UpdatePerson([FromBody] PersonEntity person)
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
    public async Task<IActionResult> CreatePerson([FromBody] PersonEntity person)
    {
        return this.Ok(await dataService.CreatePersonAsync(person, this.HttpContext.RequestAborted));
    }

    /// <summary>
    /// Delete a person from the database.
    /// </summary>
    /// <param name="id">Person id to delete</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("DeletePerson/{id}")]
    public async Task<IActionResult> DeletePerson(uint id)
    {
        await dataService.DeletePersonAsync(id, this.HttpContext.RequestAborted);

        return this.NoContent();
    }
}
