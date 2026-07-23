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
        return Ok(workforce.GetRecommendations());
    }

    [HttpPost("recommendations/generate")]
    public IActionResult GenerateRecommendation()
    {
        if (!workforce.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "Generating recommendations requires the SmartFactory database." });
        }

        return Ok(workforce.GenerateRecommendations());
    }

    /// <summary>Applies the plan — assigns available employees to the under-staffed shifts.</summary>
    [HttpPost("recommendations/apply")]
    public IActionResult ApplyRecommendation()
    {
        if (!workforce.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "Applying the shift plan requires the SmartFactory database." });
        }

        var result = workforce.ApplyRecommendations();
        return Ok(new { assigned = result.Assigned, recommendations = result.Recommendations });
    }
}
