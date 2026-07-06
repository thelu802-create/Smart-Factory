using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Services;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("warehouse")]
public sealed class WarehouseController(SampleDataService data) : ControllerBase
{
    [HttpGet("items")]
    public IActionResult GetItems([FromQuery] string? search = null)
    {
        var items = data.GetWarehouseItems();
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
        var item = data.GetWarehouseItems().FirstOrDefault(value => value.Id == itemId);
        return item is null ? NotFound(new { detail = "Warehouse item not found" }) : Ok(item);
    }
}