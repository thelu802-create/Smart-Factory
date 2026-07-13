using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Repositories;
using SmartFactory.Api.Services;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("workforce")]
public sealed class WorkforceController(WorkforceRepository workforce, SampleDataService data) : ControllerBase
{
    [HttpGet("shifts")]
    public IActionResult GetShifts()
    {
        return Ok(data.GetShiftPlans());
    }

    [HttpGet("recommendations")]
    public IActionResult GetRecommendations()
    {
        // Read live from SQLite when available so generated recommendations are reflected;
        // fall back to the startup snapshot when running on JSON demo data.
        return Ok(workforce.IsAvailable() ? workforce.GetRecommendationMessages() : data.GetRecommendationMessages());
    }

    [HttpPost("recommendations/generate")]
    public IActionResult GenerateRecommendation()
    {
        if (!workforce.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "Generating recommendations requires the SQLite database." });
        }

        return Ok(workforce.GenerateRecommendations());
    }
}
