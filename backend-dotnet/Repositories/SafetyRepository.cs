using Microsoft.Data.Sqlite;
using SmartFactory.Api.Data;
using SmartFactory.Api.Models;

namespace SmartFactory.Api.Repositories;

/// <summary>
/// Live SQLite access for the safety module. Reads alerts on every request and
/// persists resolve/escalate actions (status + action_note) so responses are
/// durable, mirroring the forms approval flow.
/// </summary>
public sealed class SafetyRepository(DbConnectionFactory connectionFactory)
{
    public bool IsAvailable() => connectionFactory.IsAvailable();

    public IReadOnlyList<SafetyAlertDto> GetAlerts()
    {
        using var connection = connectionFactory.CreateOpenConnection();
        return ReadAlerts(connection);
    }

    public SafetyAlertDto? GetAlert(string alertId)
    {
        using var connection = connectionFactory.CreateOpenConnection();
        return ReadAlerts(connection, alertId).FirstOrDefault();
    }

    /// <summary>Marks an alert resolved. Returns the updated alert, or null when the id does not exist.</summary>
    public SafetyAlertDto? Resolve(string alertId, string? note)
    {
        return Decide(alertId, "Resolved", note ?? "Resolved");
    }

    /// <summary>Escalates an alert. Returns the updated alert, or null when the id does not exist.</summary>
    public SafetyAlertDto? Escalate(string alertId, string? note)
    {
        return Decide(alertId, "Escalated", note ?? "Escalated");
    }

    private SafetyAlertDto? Decide(string alertId, string status, string actionNote)
    {
        using var connection = connectionFactory.CreateOpenConnection();

        int updated;
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "UPDATE safety_alerts SET status = $status, action_note = $note WHERE id = $id";
            command.Parameters.AddWithValue("$status", status);
            command.Parameters.AddWithValue("$note", actionNote);
            command.Parameters.AddWithValue("$id", alertId);
            updated = command.ExecuteNonQuery();
        }

        return updated == 0 ? null : ReadAlerts(connection, alertId).FirstOrDefault();
    }

    private static List<SafetyAlertDto> ReadAlerts(SqliteConnection connection, string? alertId = null)
    {
        const string baseSql = """
            SELECT s.id, s.title, s.alert_type, s.severity, a.name AS location, s.status, s.detected_at, s.description
            FROM safety_alerts s
            JOIN factory_areas a ON a.id = s.area_id
            """;

        using var command = connection.CreateCommand();
        command.CommandText = alertId is null
            ? baseSql + " ORDER BY s.detected_at DESC"
            : baseSql + " WHERE s.id = $id";
        if (alertId is not null)
        {
            command.Parameters.AddWithValue("$id", alertId);
        }

        using var reader = command.ExecuteReader();
        var alerts = new List<SafetyAlertDto>();
        while (reader.Read())
        {
            alerts.Add(new SafetyAlertDto(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetString(5),
                TimeValue(reader.GetString(6)),
                reader.GetString(7)));
        }

        return alerts;
    }

    // Matches SampleDataService.TimeValue so the frontend sees the same "HH:mm" detected time.
    private static string TimeValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Contains('T') && value.Length >= 16 ? value.Substring(11, 5) : value;
    }
}
