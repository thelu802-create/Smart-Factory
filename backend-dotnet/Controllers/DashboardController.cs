using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Services;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("dashboard")]
public sealed class DashboardController(SampleDataService data) : ControllerBase
{
    [HttpGet("summary")]
    public IActionResult GetSummary()
    {
        return Ok(new
        {
            kpis = data.GetKpis(),
            productionLines = data.GetProductionLines().Take(4),
            safetyAlerts = data.GetSafetyAlerts(),
            warehouseSignals = data.GetWarehouseItems(),
            pendingForms = data.GetFormRequests()
        });
    }

    [HttpGet("kpis")]
    public IActionResult GetKpis()
    {
        return Ok(data.GetKpis());
    }

    [HttpGet("alerts")]
    public IActionResult GetAlerts()
    {
        return Ok(data.GetSafetyAlerts());
    }
}