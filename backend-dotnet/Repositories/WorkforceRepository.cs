using Microsoft.Data.Sqlite;
using SmartFactory.Api.Data;

namespace SmartFactory.Api.Repositories;

/// <summary>
/// Live SQLite access for the workforce module. Reads AI recommendation messages
/// and generates staffing recommendations from current shift gaps, inserting them
/// into ai_recommendations so they persist and show on the Workforce page.
/// </summary>
public sealed class WorkforceRepository(DbConnectionFactory connectionFactory)
{
    // Auto-generated rows are marked with this id prefix so regenerating replaces
    // only our own rows and never touches seeded/demo recommendations.
    private const string AutoPrefix = "wf-auto-";

    public bool IsAvailable() => connectionFactory.IsAvailable();

    public IReadOnlyList<string> GetRecommendationMessages()
    {
        using var connection = connectionFactory.CreateOpenConnection();
        return ReadMessages(connection);
    }

    /// <summary>
    /// Recomputes staffing recommendations from shifts where assigned &lt; required,
    /// replacing any previously auto-generated rows. Returns the full message list.
    /// </summary>
    public IReadOnlyList<string> GenerateRecommendations()
    {
        using var connection = connectionFactory.CreateOpenConnection();
        using var transaction = connection.BeginTransaction();

        // Clear previously auto-generated recommendations so repeated clicks don't stack up.
        using (var clear = connection.CreateCommand())
        {
            clear.Transaction = transaction;
            clear.CommandText = "DELETE FROM ai_recommendations WHERE id LIKE $prefix";
            clear.Parameters.AddWithValue("$prefix", AutoPrefix + "%");
            clear.ExecuteNonQuery();
        }

        var gaps = ReadShiftGaps(connection, transaction);
        var createdAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");

        if (gaps.Count == 0)
        {
            Insert(connection, transaction, NewId(), "Staffing balanced",
                "All shifts currently meet their required worker count.",
                "No additional staffing action needed right now.", createdAt);
        }
        else
        {
            foreach (var gap in gaps)
            {
                Insert(connection, transaction, NewId(),
                    $"Staffing gap on {gap.Line} - {gap.Shift}",
                    $"Assigned {gap.Assigned}/{gap.Required} workers for the {gap.Shift} shift on {gap.Line}.",
                    $"Add {gap.Required - gap.Assigned} worker(s) or approve overtime to reach {gap.Required} required.",
                    createdAt);
            }
        }

        transaction.Commit();
        return ReadMessages(connection);
    }

    private static void Insert(SqliteConnection connection, SqliteTransaction transaction, string id, string title, string reason, string expectedImpact, string createdAt)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO ai_recommendations (id, module, title, reason, expected_impact, status, created_at, reviewed_by)
            VALUES ($id, 'Workforce', $title, $reason, $impact, 'New', $createdAt, NULL)
            """;
        command.Parameters.AddWithValue("$id", id);
        command.Parameters.AddWithValue("$title", title);
        command.Parameters.AddWithValue("$reason", reason);
        command.Parameters.AddWithValue("$impact", expectedImpact);
        command.Parameters.AddWithValue("$createdAt", createdAt);
        command.ExecuteNonQuery();
    }

    private sealed record ShiftGap(string Shift, string Line, int Required, int Assigned);

    private static List<ShiftGap> ReadShiftGaps(SqliteConnection connection, SqliteTransaction transaction)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            SELECT s.shift_name, p.name AS line_name, s.required_workers, s.assigned_workers
            FROM shift_plans s
            JOIN production_lines p ON p.id = s.line_id
            WHERE s.assigned_workers < s.required_workers
            ORDER BY (s.required_workers - s.assigned_workers) DESC
            """;

        using var reader = command.ExecuteReader();
        var gaps = new List<ShiftGap>();
        while (reader.Read())
        {
            gaps.Add(new ShiftGap(reader.GetString(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3)));
        }

        return gaps;
    }

    private static List<string> ReadMessages(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT title, expected_impact FROM ai_recommendations ORDER BY created_at DESC";

        using var reader = command.ExecuteReader();
        var messages = new List<string>();
        while (reader.Read())
        {
            messages.Add($"{reader.GetString(0)}: {reader.GetString(1)}");
        }

        return messages;
    }

    private static string NewId() => AutoPrefix + Guid.NewGuid().ToString("N");
}
