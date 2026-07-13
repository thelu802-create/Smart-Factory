using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Models;
using SmartFactory.Api.Models.Requests;
using SmartFactory.Api.Repositories;
using SmartFactory.Api.Services;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("warehouse")]
public sealed class WarehouseController(WarehouseRepository warehouse, SampleDataService data) : ControllerBase
{
    [HttpGet("items")]
    public IActionResult GetItems([FromQuery] string? search = null)
    {
        // Read live from SQLite when available so stock movements are reflected;
        // fall back to the startup snapshot when running on JSON demo data.
        IReadOnlyList<WarehouseItemDto> items = warehouse.IsAvailable()
            ? warehouse.GetItems()
            : data.GetWarehouseItems();

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
        var item = warehouse.IsAvailable()
            ? warehouse.GetItem(itemId)
            : data.GetWarehouseItems().FirstOrDefault(value => value.Id == itemId);
        return item is null ? NotFound(new { detail = "Warehouse item not found" }) : Ok(item);
    }

    [HttpPost("items/{itemId}/move")]
    public IActionResult MoveItem(string itemId, [FromBody] MoveWarehouseRequest request)
    {
        if (!warehouse.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "Warehouse movements require the SQLite database." });
        }

        var result = warehouse.Move(itemId, request?.MovementType, request?.Quantity ?? 0, request?.Note);
        return result.Status switch
        {
            "not_found" => NotFound(new { detail = result.Error }),
            "invalid" => BadRequest(new { detail = result.Error }),
            _ => Ok(result.Item)
        };
    }
}
