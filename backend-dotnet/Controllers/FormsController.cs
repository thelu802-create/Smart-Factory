using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Models.Requests;
using SmartFactory.Api.Repositories;
using SmartFactory.Api.Services;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("forms")]
public sealed class FormsController(FormsRepository forms, SampleDataService data) : ControllerBase
{
    [HttpGet]
    public IActionResult GetForms()
    {
        // Read live from SQLite when available so approve/reject actions are reflected;
        // fall back to the startup snapshot when running on JSON demo data.
        return Ok(forms.IsAvailable() ? forms.GetForms() : data.GetFormRequests());
    }

    [HttpPost("{formId}/approve")]
    public IActionResult ApproveForm(string formId, [FromBody] FormDecisionRequest? request)
    {
        if (!forms.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "Form actions require the SQLite database." });
        }

        var updated = forms.Approve(formId, request?.Note);
        return updated is null ? NotFound(new { detail = "Form request not found" }) : Ok(updated);
    }

    [HttpPost("{formId}/reject")]
    public IActionResult RejectForm(string formId, [FromBody] FormDecisionRequest? request)
    {
        if (!forms.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "Form actions require the SQLite database." });
        }

        var updated = forms.Reject(formId, request?.Note);
        return updated is null ? NotFound(new { detail = "Form request not found" }) : Ok(updated);
    }
}
