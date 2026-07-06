using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Services;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("cameras")]
public sealed class CamerasController(SampleDataService data) : ControllerBase
{
    [HttpGet]
    public IActionResult GetCameras()
    {
        return Ok(new[]
        {
            new { id = "cam-01", name = "Robot Cell 2", status = "Active" },
            new { id = "cam-02", name = "Warehouse Zone C", status = "Active" },
            new { id = "cam-03", name = "Line C", status = "Active" },
            new { id = "cam-04", name = "Storage Room B", status = "Active" }
        });
    }

    [HttpGet("events")]
    public IActionResult GetEvents()
    {
        return Ok(data.GetCameraEvents());
    }
}