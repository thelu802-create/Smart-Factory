using Microsoft.Data.Sqlite;
using SmartFactory.Api.Data;
using SmartFactory.Api.Models;

namespace SmartFactory.Api.Repositories;

/// <summary>
/// Live SQLite access for the electronic forms module. Unlike the startup
/// snapshot in SampleDataService, this reads and writes on every request so
/// approve/reject actions are persisted and immediately visible on GET /forms.
/// </summary>
public sealed class FormsRepository(DbConnectionFactory connectionFactory)
{
    public bool IsAvailable() => connectionFactory.IsAvailable();

    public IReadOnlyList<FormRequestDto> GetForms()
    {
        using var connection = connectionFactory.CreateOpenConnection();
        return ReadForms(connection);
    }

    /// <summary>Approves a form. Returns the updated form, or null when the id does not exist.</summary>
    public FormRequestDto? Approve(string formId, string? note)
    {
        return Decide(formId, "Approved", approvedAt: UtcNowIso(), rejectionReason: null, stepNote: note ?? "Approved");
    }

    /// <summary>Rejects a form. Returns the updated form, or null when the id does not exist.</summary>
    public FormRequestDto? Reject(string formId, string? reason)
    {
        return Decide(formId, "Rejected", approvedAt: null, rejectionReason: reason, stepNote: reason ?? "Rejected");
    }

    private FormRequestDto? Decide(string formId, string status, string? approvedAt, string? rejectionReason, string stepNote)
    {
        using var connection = connectionFactory.CreateOpenConnection();
        using var transaction = connection.BeginTransaction();

        var actionAt = UtcNowIso();

        var updatedForms = Execute(
            connection,
            transaction,
            "UPDATE form_requests SET status = $status, approved_at = $approvedAt, rejection_reason = $reason WHERE id = $id",
            command =>
            {
                command.Parameters.AddWithValue("$status", status);
                command.Parameters.AddWithValue("$approvedAt", (object?)approvedAt ?? DBNull.Value);
                command.Parameters.AddWithValue("$reason", (object?)rejectionReason ?? DBNull.Value);
                command.Parameters.AddWithValue("$id", formId);
            });

        if (updatedForms == 0)
        {
            transaction.Rollback();
            return null;
        }

        // Advance the approval workflow steps for this form so form_approval_steps stays in sync.
        Execute(
            connection,
            transaction,
            "UPDATE form_approval_steps SET status = $status, action_at = $actionAt, note = $note WHERE form_id = $id",
            command =>
            {
                command.Parameters.AddWithValue("$status", status);
                command.Parameters.AddWithValue("$actionAt", actionAt);
                command.Parameters.AddWithValue("$note", stepNote);
                command.Parameters.AddWithValue("$id", formId);
            });

        transaction.Commit();

        return ReadForms(connection, formId).FirstOrDefault();
    }

    private static List<FormRequestDto> ReadForms(SqliteConnection connection, string? formId = null)
    {
        const string baseSql = """
            SELECT f.id, f.form_type, u.full_name AS requester, u.department, f.status, f.submitted_at, f.summary
            FROM form_requests f
            JOIN users u ON u.id = f.requester_id
            """;

        using var command = connection.CreateCommand();
        command.CommandText = formId is null
            ? baseSql + " ORDER BY f.submitted_at DESC"
            : baseSql + " WHERE f.id = $id";
        if (formId is not null)
        {
            command.Parameters.AddWithValue("$id", formId);
        }

        using var reader = command.ExecuteReader();
        var forms = new List<FormRequestDto>();
        while (reader.Read())
        {
            forms.Add(new FormRequestDto(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                TimeValue(reader.GetString(5)),
                reader.GetString(6)));
        }

        return forms;
    }

    private static int Execute(SqliteConnection connection, SqliteTransaction transaction, string sql, Action<SqliteCommand> bind)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        bind(command);
        return command.ExecuteNonQuery();
    }

    private static string UtcNowIso() => DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");

    // Matches SampleDataService.TimeValue so the frontend sees the same "HH:mm" submitted time.
    private static string TimeValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Contains('T') && value.Length >= 16 ? value.Substring(11, 5) : value;
    }
}
