using Microsoft.EntityFrameworkCore;
using SmartFactory.Api.Data;
using SmartFactory.Api.Models;

namespace SmartFactory.Api.Repositories;

/// <summary>
/// EF Core access for the notifications module. Reads notifications on every
/// request and persists the mark-as-read action.
/// </summary>
public sealed class NotificationsRepository(SmartFactoryDbContext context)
{
    public bool IsAvailable() => context.Database.CanConnect();

    public IReadOnlyList<NotificationDto> GetNotifications()
    {
        return context.Notifications
            .OrderByDescending(notification => notification.CreatedAt)
            .AsEnumerable()
            .Select(ToDto)
            .ToList();
    }

    /// <summary>Marks a notification read. Returns the updated row, or null when the id does not exist.</summary>
    public NotificationDto? MarkRead(string notificationId)
    {
        var notification = context.Notifications.FirstOrDefault(item => item.Id == notificationId);
        if (notification is null)
        {
            return null;
        }

        notification.Status = "Read";
        context.SaveChanges();
        return ToDto(notification);
    }

    private static NotificationDto ToDto(Models.Database.NotificationEntity notification) => new(
        notification.Id,
        notification.Title,
        notification.NotificationType,
        notification.Severity,
        notification.Status,
        TimeValue(notification.CreatedAt));

    private static string TimeValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Contains('T') && value.Length >= 16 ? value.Substring(11, 5) : value;
    }
}
