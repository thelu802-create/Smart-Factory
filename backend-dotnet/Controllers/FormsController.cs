using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Documents;
using SmartFactory.Api.Models.Requests;
using SmartFactory.Api.Repositories;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("forms")]
public sealed class FormsController(FormsRepository forms) : ControllerBase
{
    [HttpGet]
    public IActionResult GetForms()
    {
        return Ok(forms.GetForms());
    }

    /// <summary>Exports a form request as a printable PDF request form.</summary>
    [HttpGet("{formId}/export")]
    public IActionResult ExportForm(string formId)
    {
        var form = forms.GetForms().FirstOrDefault(item => item.Id == formId);
        if (form is null)
        {
            return NotFound(new { detail = "Form request not found" });
        }

        var pdf = FormPdfDocument.Generate(form, DateTime.Now);
        return File(pdf, "application/pdf", $"phieu-{formId}.pdf");
    }

    [HttpPost]
    public IActionResult CreateForm([FromBody] CreateFormRequest? request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.FormType) || string.IsNullOrWhiteSpace(request.Summary))
        {
            return BadRequest(new { detail = "formType and summary are required." });
        }

        if (!forms.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "Form actions require the SmartFactory database." });
        }

        var created = forms.Create(request.FormType, request.RequesterId, request.Summary);
        return created is null
            ? BadRequest(new { detail = "Requester not found." })
            : Created($"/forms/{created.Id}", created);
    }

    /// <summary>Creates a "Warehouse Borrow" form — the borrowed stock is deducted on approval.</summary>
    [HttpPost("stock-borrow")]
    public IActionResult CreateStockBorrow([FromBody] StockBorrowRequest? request)
    {
        if (!forms.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "Form actions require the SmartFactory database." });
        }

        var result = forms.CreateStockBorrow(request?.RequesterId, request?.ItemId, request?.Quantity ?? 0, request?.Note);
        return result.Status == "ok"
            ? Created($"/forms/{result.Form!.Id}", result.Form)
            : BadRequest(new { detail = result.Error });
    }

    [HttpPost("{formId}/approve")]
    public IActionResult ApproveForm(string formId, [FromBody] FormDecisionRequest? request)
    {
        if (!forms.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "Form actions require the SmartFactory database." });
        }

        var result = forms.Approve(formId, request?.Note);
        return result.Status switch
        {
            "not_found" => NotFound(new { detail = result.Error }),
            "invalid" => BadRequest(new { detail = result.Error }),
            _ => Ok(result.Form)
        };
    }

    /// <summary>Approves several forms in one request. Borrow forms deduct their stock; one failure does not block the rest.</summary>
    [HttpPost("approve-batch")]
    public IActionResult ApproveBatch([FromBody] BatchDecisionRequest? request)
    {
        if (request?.Ids is null || request.Ids.Count == 0)
        {
            return BadRequest(new { detail = "ids is required." });
        }

        if (!forms.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "Form actions require the SmartFactory database." });
        }

        var results = forms.ApproveMany(request.Ids, request.Note);
        return Ok(BatchResponse(results));
    }

    /// <summary>Rejects several forms in one request.</summary>
    [HttpPost("reject-batch")]
    public IActionResult RejectBatch([FromBody] BatchDecisionRequest? request)
    {
        if (request?.Ids is null || request.Ids.Count == 0)
        {
            return BadRequest(new { detail = "ids is required." });
        }

        if (!forms.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "Form actions require the SmartFactory database." });
        }

        var results = forms.RejectMany(request.Ids, request.Note);
        return Ok(BatchResponse(results));
    }

    private static object BatchResponse(IReadOnlyList<FormsRepository.BatchItemResult> results) => new
    {
        total = results.Count,
        succeeded = results.Count(item => item.Status == "ok"),
        failed = results.Count(item => item.Status != "ok"),
        results
    };

    [HttpPost("{formId}/reject")]
    public IActionResult RejectForm(string formId, [FromBody] FormDecisionRequest? request)
    {
        if (!forms.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "Form actions require the SmartFactory database." });
        }

        var updated = forms.Reject(formId, request?.Note);
        return updated is null ? NotFound(new { detail = "Form request not found" }) : Ok(updated);
    }
}
