using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Services;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("production")]
public sealed class ProductionController(SampleDataService data) : ControllerBase
{
    [HttpGet("lines")]
    public IActionResult GetLines()
    {
        return Ok(data.GetProductionLines());
    }

    [HttpGet("lines/{lineId}")]
    public IActionResult GetLine(string lineId)
    {
        var line = data.GetProductionLines().FirstOrDefault(item => item.Id == lineId);
        return line is null ? NotFound(new { detail = "Production line not found" }) : Ok(line);
    }
}