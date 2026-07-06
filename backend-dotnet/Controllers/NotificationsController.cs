using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Services;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("notifications")]
public sealed class NotificationsController(SampleDataService data) : ControllerBase
{
    [HttpGet]
    public IActionResult GetNotifications()
    {
        return Ok(data.GetNotifications());
    }

    [HttpPost("{notificationId}/read")]
    public IActionResult MarkNotificationRead(string notificationId)
    {
        return Ok(new { id = notificationId, status = "Read" });
    }
}