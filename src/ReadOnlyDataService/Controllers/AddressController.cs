using Microsoft.AspNetCore.Mvc;
using ReadOnlyDataService.Services;
using Shared.Infrastructure.Database.Entities;
using System.Net;
using System.Net.Mime;

namespace ReadOnlyDataService.Controllers;

/// <summary>
/// Address controller
/// </summary>
[ApiController]
[Route("addresses")]
public sealed class AddressController(IReadOnlyDatabaseService dbService) : ControllerBase
{
    /// <summary>
    /// Retrieves all addresses from the database.
    /// </summary>
    /// <returns>Collection of addresses</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AddressEntity>), (int)HttpStatusCode.OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<AddressEntity>>> GetAllAddresses()
    {
        return this.Ok(await dbService.GetAddressesAsync(this.HttpContext.RequestAborted));
    }

    /// <summary>
    /// Retrieves an address by its unique identifier.
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <returns>Address if found</returns>
    [HttpGet("getbyid/{id}")]
    [ProducesResponseType(typeof(AddressEntity), (int)HttpStatusCode.OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<AddressEntity>> GetAddressById(uint id)
    {
        if (id < 1)
        {
            return this.BadRequest("Invalid id!");
        }

        var result = await dbService.GetAddressByIdAsync(id, this.HttpContext.RequestAborted);

        return result is null
            ? this.NotFound()
            : this.Ok(result);
    }
}
