using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Models.Requests;
using SmartFactory.Api.Repositories;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("cameras")]
public sealed class CamerasController(CameraRepository cameras) : ControllerBase
{
    [HttpGet]
    public IActionResult GetCameras()
    {
        return Ok(cameras.GetCameras());
    }

    [HttpGet("events")]
    public IActionResult GetEvents()
    {
        return Ok(cameras.GetEvents());
    }

    [HttpPost("detect")]
    public IActionResult Detect([FromBody] CameraDetectionRequest? request)
    {
        if (!cameras.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "Camera detection requires the SmartFactory database." });
        }

        var result = cameras.Detect(request?.CameraCode, request?.EventType, request?.Severity, request?.Confidence);
        return result.Status == "invalid"
            ? BadRequest(new { detail = result.Error })
            : Ok(new { @event = result.Event, alertRaised = result.AlertRaised, alertId = result.AlertId });
    }
}
