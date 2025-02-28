using DatabaseService.Extensions;
using DatabaseService.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Infrastructure.Database.Entities;

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
    public async Task<IActionResult> GetAllAddresses()
    {
        return this.Ok(await dataService.GetAddressesAsync(this.HttpContext.RequestAborted));
    }

    /// <summary>
    /// Retrieves an address by its unique identifier.
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns>The found address or not found</returns>
    [HttpGet("GetAddressById/{id}")]
    public async Task<IActionResult> GetAddressById(uint id)
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
    public async Task<IActionResult> CreateAddress([FromBody] AddressEntity address)
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
    public async Task<IActionResult> UpdateAddress([FromBody] AddressEntity address)
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
    public async Task<IActionResult> DeleteAddress(uint id)
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
