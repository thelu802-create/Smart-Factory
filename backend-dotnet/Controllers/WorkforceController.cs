using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Models.Requests;
using SmartFactory.Api.Services;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("workforce")]
public sealed class WorkforceController(SampleDataService data) : ControllerBase
{
    [HttpGet("shifts")]
    public IActionResult GetShifts()
    {
        return Ok(data.GetShiftPlans());
    }

    [HttpGet("recommendations")]
    public IActionResult GetRecommendations()
    {
        return Ok(data.GetRecommendationMessages());
    }

    [HttpPost("recommendations/generate")]
    public IActionResult GenerateRecommendation([FromBody] RecommendationRequest payload)
    {
        var workerGap = Math.Max(0, 20 - payload.AvailableWorkers);
        var overtimeHours = payload.TargetOutput > 12000 || workerGap > 0 ? 1.5 : 0;

        return Ok(new
        {
            targetOutput = payload.TargetOutput,
            availableWorkers = payload.AvailableWorkers,
            workerGap,
            overtimeHours,
            recommendations = data.GetRecommendationMessages().Take(2)
        });
    }
}