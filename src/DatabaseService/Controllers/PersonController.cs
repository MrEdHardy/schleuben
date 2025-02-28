using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Configuration.Json;
using Shared.Infrastructure.Database;
using Shared.Infrastructure.Database.Entities;
using System.Text.Json;

namespace DatabaseService.Controllers;

/// <summary>
/// Person controller
/// </summary>
/// <param name="dbContextFactory"></param>
/// <param name="logger"></param>
[ApiController]
[Route("persons")]
public class PersonController(
    IDbContextFactory<DatabaseContext> dbContextFactory,
    ILogger<PersonController> logger,
    CancellationTokenSource source,
    JsonSerializerOptionsProvider settingsProvider) : ControllerBase
{
    private readonly CancellationToken cancellationToken = source.Token;

    /// <summary>
    /// Test action
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetHallo")]
    public string GetHallo()
    {
        return "Hallo du Penner";
    }

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
    public async Task<string> GetAllPersons()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(this.cancellationToken);

        var allPeople = await GetBasePersonQuery(dbContext)
            .ToArrayAsync(this.cancellationToken);

        return JsonSerializer.Serialize(allPeople, settingsProvider.GetOptions());
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

            logger.LogWarning("Invalid id was provided!");

            return this.BadRequest(reason);
        }

        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync(this.cancellationToken);

            var result = await GetBasePersonQuery(dbContext)
                .FirstOrDefaultAsync(p => p.Id == id, this.cancellationToken);

            return result is not null
                ? this.Ok(JsonSerializer.Serialize(result, settingsProvider.GetOptions()))
                : this.NotFound($"No person with given id {id} was found!");
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while accessing the database.");

            return this.StatusCode(
                StatusCodes.Status500InternalServerError,
                "An error occured while processing the request");
        }
    }

    private static IQueryable<PersonEntity> GetBasePersonQuery(DatabaseContext dbContext)
    {
        return dbContext.People
            .AsNoTracking()
            .Include(p => p.Addresses)
            .Include(p => p.TelephoneConnections)
            .AsSplitQuery();
    }
}
