using Microsoft.Data.Sqlite;
using SmartFactory.Api.Models;

using SmartFactory.Api.Data;

namespace SmartFactory.Api.Repositories;

/// <summary>
/// Live SQLite access for the notifications module. Reads notifications on every
/// request and persists the mark-as-read action, mirroring the forms/safety flows.
/// </summary>
public sealed class NotificationsRepository(DbConnectionFactory connectionFactory)
{
    public bool IsAvailable() => connectionFactory.IsAvailable();

    public IReadOnlyList<NotificationDto> GetNotifications()
    {
        using var connection = connectionFactory.CreateOpenConnection();
        return ReadNotifications(connection);
    }

    /// <summary>Marks a notification read. Returns the updated row, or null when the id does not exist.</summary>
    public NotificationDto? MarkRead(string notificationId)
    {
        using var connection = connectionFactory.CreateOpenConnection();

        int updated;
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "UPDATE notifications SET status = 'Read' WHERE id = $id";
            command.Parameters.AddWithValue("$id", notificationId);
            updated = command.ExecuteNonQuery();
        }

        return updated == 0 ? null : ReadNotifications(connection, notificationId).FirstOrDefault();
    }

    private static List<NotificationDto> ReadNotifications(SqliteConnection connection, string? notificationId = null)
    {
        const string baseSql = "SELECT id, title, notification_type, severity, status, created_at FROM notifications";

        using var command = connection.CreateCommand();
        command.CommandText = notificationId is null
            ? baseSql + " ORDER BY created_at DESC"
            : baseSql + " WHERE id = $id";
        if (notificationId is not null)
        {
            command.Parameters.AddWithValue("$id", notificationId);
        }

        using var reader = command.ExecuteReader();
        var notifications = new List<NotificationDto>();
        while (reader.Read())
        {
            notifications.Add(new NotificationDto(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                TimeValue(reader.GetString(5))));
        }

        return notifications;
    }

    // Matches SampleDataService.TimeValue so the frontend sees the same "HH:mm" time.
    private static string TimeValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Contains('T') && value.Length >= 16 ? value.Substring(11, 5) : value;
    }
}
