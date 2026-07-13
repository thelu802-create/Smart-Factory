using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Models.Requests;
using SmartFactory.Api.Repositories;
using SmartFactory.Api.Services;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("safety")]
public sealed class SafetyController(SafetyRepository safety, SampleDataService data) : ControllerBase
{
    [HttpGet("alerts")]
    public IActionResult GetAlerts()
    {
        // Read live from SQLite when available so resolve/escalate actions are reflected;
        // fall back to the startup snapshot when running on JSON demo data.
        return Ok(safety.IsAvailable() ? safety.GetAlerts() : data.GetSafetyAlerts());
    }

    [HttpGet("alerts/{alertId}")]
    public IActionResult GetAlert(string alertId)
    {
        var alert = safety.IsAvailable()
            ? safety.GetAlert(alertId)
            : data.GetSafetyAlerts().FirstOrDefault(item => item.Id == alertId);
        return alert is null ? NotFound(new { detail = "Safety alert not found" }) : Ok(alert);
    }

    [HttpPost("alerts/{alertId}/resolve")]
    public IActionResult ResolveAlert(string alertId, [FromBody] AlertActionRequest? request)
    {
        if (!safety.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "Alert actions require the SQLite database." });
        }

        var updated = safety.Resolve(alertId, request?.Note);
        return updated is null ? NotFound(new { detail = "Safety alert not found" }) : Ok(updated);
    }

    [HttpPost("alerts/{alertId}/escalate")]
    public IActionResult EscalateAlert(string alertId, [FromBody] AlertActionRequest? request)
    {
        if (!safety.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "Alert actions require the SQLite database." });
        }

        var updated = safety.Escalate(alertId, request?.Note);
        return updated is null ? NotFound(new { detail = "Safety alert not found" }) : Ok(updated);
    }
}
