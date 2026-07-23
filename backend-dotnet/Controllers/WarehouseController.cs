using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Models;
using SmartFactory.Api.Models.Requests;
using SmartFactory.Api.Repositories;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("warehouse")]
public sealed class WarehouseController(WarehouseRepository warehouse) : ControllerBase
{
    [HttpGet("items")]
    public IActionResult GetItems([FromQuery] string? search = null)
    {
        IReadOnlyList<WarehouseItemDto> items = warehouse.GetItems();

        if (string.IsNullOrWhiteSpace(search))
        {
            return Ok(items);
        }

        var normalizedSearch = search.ToLowerInvariant();
        return Ok(items.Where(item =>
            item.ItemCode.ToLowerInvariant().Contains(normalizedSearch) ||
            item.ItemName.ToLowerInvariant().Contains(normalizedSearch) ||
            item.BatchCode.ToLowerInvariant().Contains(normalizedSearch) ||
            item.IoId.ToLowerInvariant().Contains(normalizedSearch) ||
            item.IoCode.ToLowerInvariant().Contains(normalizedSearch) ||
            item.Bu.ToLowerInvariant().Contains(normalizedSearch)));
    }

    [HttpGet("items/{itemId}")]
    public IActionResult GetItem(string itemId)
    {
        var item = warehouse.GetItem(itemId);
        return item is null ? NotFound(new { detail = "Warehouse item not found" }) : Ok(item);
    }

    [HttpGet("zones")]
    public IActionResult GetZones()
    {
        return Ok(warehouse.GetZones());
    }

    [HttpGet("items/{itemId}/movements")]
    public IActionResult GetMovements(string itemId)
    {
        return Ok(warehouse.GetMovements(itemId));
    }

    [HttpPost("items/{itemId}/move")]
    public IActionResult MoveItem(string itemId, [FromBody] MoveWarehouseRequest request)
    {
        if (!warehouse.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "Warehouse movements require the SmartFactory database." });
        }

        var result = warehouse.Move(itemId, request?.MovementType, request?.Quantity ?? 0, request?.ToZoneId, request?.Note);
        return result.Status switch
        {
            "not_found" => NotFound(new { detail = result.Error }),
            "invalid" => BadRequest(new { detail = result.Error }),
            _ => Ok(result.Item)
        };
    }
}
