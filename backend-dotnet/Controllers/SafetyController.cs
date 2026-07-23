using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Models.Requests;
using SmartFactory.Api.Repositories;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("safety")]
public sealed class SafetyController(SafetyRepository safety) : ControllerBase
{
    [HttpGet("alerts")]
    public IActionResult GetAlerts()
    {
        return Ok(safety.GetAlerts());
    }

    [HttpGet("alerts/{alertId}")]
    public IActionResult GetAlert(string alertId)
    {
        var alert = safety.GetAlert(alertId);
        return alert is null ? NotFound(new { detail = "Safety alert not found" }) : Ok(alert);
    }

    [HttpPost("alerts/{alertId}/resolve")]
    public IActionResult ResolveAlert(string alertId, [FromBody] AlertActionRequest? request)
    {
        if (!safety.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "Alert actions require the SmartFactory database." });
        }

        var updated = safety.Resolve(alertId, request?.Note);
        return updated is null ? NotFound(new { detail = "Safety alert not found" }) : Ok(updated);
    }

    [HttpPost("alerts/{alertId}/escalate")]
    public IActionResult EscalateAlert(string alertId, [FromBody] AlertActionRequest? request)
    {
        if (!safety.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "Alert actions require the SmartFactory database." });
        }

        var updated = safety.Escalate(alertId, request?.Note);
        return updated is null ? NotFound(new { detail = "Safety alert not found" }) : Ok(updated);
    }
}
