using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Services;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("forms")]
public sealed class FormsController(SampleDataService data) : ControllerBase
{
    [HttpGet]
    public IActionResult GetForms()
    {
        return Ok(data.GetFormRequests());
    }

    [HttpPost("{formId}/approve")]
    public IActionResult ApproveForm(string formId)
    {
        return Ok(new { id = formId, status = "Approved" });
    }

    [HttpPost("{formId}/reject")]
    public IActionResult RejectForm(string formId)
    {
        return Ok(new { id = formId, status = "Rejected" });
    }
}