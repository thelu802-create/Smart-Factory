using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Services;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("safety")]
public sealed class SafetyController(SampleDataService data) : ControllerBase
{
    [HttpGet("alerts")]
    public IActionResult GetAlerts()
    {
        return Ok(data.GetSafetyAlerts());
    }

    [HttpGet("alerts/{alertId}")]
    public IActionResult GetAlert(string alertId)
    {
        var alert = data.GetSafetyAlerts().FirstOrDefault(item => item.Id == alertId);
        return alert is null ? NotFound(new { detail = "Safety alert not found" }) : Ok(alert);
    }

    [HttpPost("alerts/{alertId}/resolve")]
    public IActionResult ResolveAlert(string alertId)
    {
        return Ok(new { id = alertId, status = "Resolved" });
    }

    [HttpPost("alerts/{alertId}/escalate")]
    public IActionResult EscalateAlert(string alertId)
    {
        return Ok(new { id = alertId, status = "Escalated" });
    }
}