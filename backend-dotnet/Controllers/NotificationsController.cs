using Microsoft.AspNetCore.Mvc;
using SmartFactory.Api.Repositories;

namespace SmartFactory.Api.Controllers;

[ApiController]
[Route("notifications")]
public sealed class NotificationsController(NotificationsRepository notifications) : ControllerBase
{
    [HttpGet]
    public IActionResult GetNotifications()
    {
        return Ok(notifications.GetNotifications());
    }

    [HttpPost("{notificationId}/read")]
    public IActionResult MarkNotificationRead(string notificationId)
    {
        if (!notifications.IsAvailable())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { detail = "Notification actions require the SmartFactory database." });
        }

        var updated = notifications.MarkRead(notificationId);
        return updated is null ? NotFound(new { detail = "Notification not found" }) : Ok(updated);
    }
}
