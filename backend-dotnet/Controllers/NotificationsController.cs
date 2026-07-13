using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Repositories;
using SmartFactory.Api.Services;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("notifications")]
public sealed class NotificationsController(NotificationsRepository notifications, SampleDataService data) : ControllerBase
{
    [HttpGet]
    public IActionResult GetNotifications()
    {
        // Read live from SQLite when available so the mark-as-read action is reflected;
        // fall back to the startup snapshot when running on JSON demo data.
        return Ok(notifications.IsAvailable() ? notifications.GetNotifications() : data.GetNotifications());
    }

    [HttpPost("{notificationId}/read")]
    public IActionResult MarkNotificationRead(string notificationId)
    {
        if (!notifications.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "Notification actions require the SQLite database." });
        }

        var updated = notifications.MarkRead(notificationId);
        return updated is null ? NotFound(new { detail = "Notification not found" }) : Ok(updated);
    }
}
