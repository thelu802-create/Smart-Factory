using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Services;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("reports")]
public sealed class ReportsController(SampleDataService data) : ControllerBase
{
    [HttpGet("production")]
    public IActionResult GetProductionReport()
    {
        return Ok(new { type = "production", items = data.GetProductionLines() });
    }

    [HttpGet("warehouse")]
    public IActionResult GetWarehouseReport()
    {
        return Ok(new { type = "warehouse", items = data.GetWarehouseItems() });
    }

    [HttpGet("safety")]
    public IActionResult GetSafetyReport()
    {
        return Ok(new { type = "safety", items = data.GetSafetyAlerts() });
    }

    [HttpGet("workforce")]
    public IActionResult GetWorkforceReport()
    {
        return Ok(new { type = "workforce", items = data.GetShiftPlans() });
    }

    [HttpGet("forms")]
    public IActionResult GetFormsReport()
    {
        return Ok(new { type = "forms", items = data.GetFormRequests() });
    }
}