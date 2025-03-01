using DatabaseService.Extensions;
using DatabaseService.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Infrastructure.Database.Entities;
using System.Net;
using System.Net.Mime;

namespace DatabaseService.Controllers;

/// <summary>
/// Address controller
/// </summary>
/// <param name="dataService">Data service</param>
/// <param name="logger">Logger</param>
[ApiController]
[Route("addresses")]
public sealed class AddressController(
    IAddressDataService dataService,
    ILogger<AddressController> logger) : ControllerBase
{
    /// <summary>
    /// Retrieves all addresses from the database.
    /// </summary>
    /// <returns>An enumerable of addresses</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AddressEntity>), (int)HttpStatusCode.OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<AddressEntity>>> GetAllAddresses()
    {
        return this.Ok(await dataService.GetAddressesAsync(this.HttpContext.RequestAborted));
    }

    /// <summary>
    /// Retrieves an address by its unique identifier.
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns>The found address or not found</returns>
    [HttpGet("GetAddressById/{id}")]
    [ProducesResponseType(typeof(AddressEntity), (int)HttpStatusCode.OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<AddressEntity>> GetAddressById(uint id)
    {
        var idHandleResult = this.HandleId(logger, id);

        if (idHandleResult is not null)
        {
            return idHandleResult;
        }

        var result = await dataService.GetAddressByIdAsync(id, this.HttpContext.RequestAborted);

        return result is null
            ? this.NotFound()
            : this.Ok(result);
    }

    /// <summary>
    /// Creates a new address.
    /// </summary>
    /// <param name="address">Address to be created</param>
    /// <returns>The newly created entity</returns>
    [HttpPut("CreateAddress")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(AddressEntity), (int)HttpStatusCode.Created, MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<AddressEntity>> CreateAddress([FromBody] AddressEntity address)
    {
        var result = await dataService.CreateAddressAsync(address, this.HttpContext.RequestAborted);

        return this.CreatedAtAction(nameof(CreateAddress), new { result.Id }, result);
    }

    /// <summary>
    /// Updates an address.
    /// </summary>
    /// <param name="address">Address to be updated</param>
    /// <returns>No content</returns>
    [HttpPatch("UpdateAddress")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> UpdateAddress([FromBody] AddressEntity address)
    {
        await dataService.UpdateAddressAsync(address, this.HttpContext.RequestAborted);

        return this.NoContent();
    }

    /// <summary>
    /// Deletes an address.
    /// </summary>
    /// <param name="id">Id to be deleted</param>
    /// <returns>No content</returns>
    [HttpDelete("DeleteAddress/{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> DeleteAddress(uint id)
    {
        var idHandleResult = this.HandleId(logger, id);

        if (idHandleResult is not null)
        {
            return idHandleResult;
        }

        await dataService.DeleteAddressAsync(id, this.HttpContext.RequestAborted);

        return this.NoContent();
    }
}
