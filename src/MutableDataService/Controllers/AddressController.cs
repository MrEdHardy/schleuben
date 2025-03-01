using Microsoft.AspNetCore.Mvc;
using MutableDataService.Services;
using Shared.Infrastructure.Database.Entities;
using System.Net;
using System.Net.Mime;

namespace MutableDataService.Controllers;

/// <summary>
/// Address controller
/// </summary>
/// <param name="dataService">Database service</param>
[ApiController]
[Route("addresses")]
public sealed class AddressController(IMutableDatabaseService dataService) : ControllerBase
{
    /// <summary>
    /// Creates a new address.
    /// </summary>
    /// <param name="address">Address to be created</param>
    /// <returns>Address</returns>
    [HttpPut("Create")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(AddressEntity), (int)HttpStatusCode.OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<AddressEntity>> CreateAddress([FromBody] AddressEntity address)
    {
        var result = await dataService.CreateAddressAsync(address, this.HttpContext.RequestAborted);

        return this.Ok(result);
    }

    /// <summary>
    /// Updates an address.
    /// </summary>
    /// <param name="address">Address to update</param>
    /// <returns>No content if successful</returns>
    [HttpPatch("Update")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> UpdateAddress([FromBody] AddressEntity address)
    {
        await dataService.UpdateAddressAsync(address, this.HttpContext.RequestAborted);

        return this.NoContent();
    }

    /// <summary>
    /// Deletes an address by its ID.
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("Delete/{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> DeleteAddress(uint id)
    {
        await dataService.DeleteAddressAsync(id, this.HttpContext.RequestAborted);

        return this.NoContent();
    }
}
