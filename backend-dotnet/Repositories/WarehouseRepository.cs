using Microsoft.Data.Sqlite;
using SmartFactory.Api.Data;
using SmartFactory.Api.Models;

namespace SmartFactory.Api.Repositories;

/// <summary>
/// Live SQLite access for the warehouse module. Records stock movements as a
/// multi-table transaction: it inserts a goods_movements row and adjusts the
/// item's quantity atomically, with validation (quantity &gt; 0, no overselling).
/// </summary>
public sealed class WarehouseRepository(DbConnectionFactory connectionFactory)
{
    /// <summary>Result of a move: Status is "ok", "not_found", or "invalid".</summary>
    public sealed record MoveResult(string Status, string? Error, WarehouseItemDto? Item);

    public bool IsAvailable() => connectionFactory.IsAvailable();

    public IReadOnlyList<WarehouseItemDto> GetItems()
    {
        using var connection = connectionFactory.CreateOpenConnection();
        return ReadItems(connection);
    }

    public WarehouseItemDto? GetItem(string itemId)
    {
        using var connection = connectionFactory.CreateOpenConnection();
        return ReadItems(connection, itemId).FirstOrDefault();
    }

    public MoveResult Move(string itemId, string? movementType, int quantity, string? note)
    {
        var type = (movementType ?? string.Empty).Trim();
        if (type != "Import" && type != "Export")
        {
            return new MoveResult("invalid", "movementType must be 'Import' or 'Export'.", null);
        }

        if (quantity <= 0)
        {
            return new MoveResult("invalid", "quantity must be greater than 0.", null);
        }

        using var connection = connectionFactory.CreateOpenConnection();
        using var transaction = connection.BeginTransaction();

        // Read current state under the transaction.
        int currentQuantity;
        string zoneId;
        using (var read = connection.CreateCommand())
        {
            read.Transaction = transaction;
            read.CommandText = "SELECT quantity, zone_id FROM warehouse_items WHERE id = $id";
            read.Parameters.AddWithValue("$id", itemId);
            using var reader = read.ExecuteReader();
            if (!reader.Read())
            {
                return new MoveResult("not_found", "Warehouse item not found.", null);
            }

            currentQuantity = reader.GetInt32(0);
            zoneId = reader.GetString(1);
        }

        if (type == "Export" && quantity > currentQuantity)
        {
            return new MoveResult("invalid", $"Export quantity ({quantity}) exceeds available stock ({currentQuantity}).", null);
        }

        var newQuantity = type == "Import" ? currentQuantity + quantity : currentQuantity - quantity;
        var movedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
        var movedBy = FirstUserId(connection, transaction);
        // Import arrives into the item's zone (no source); Export leaves from it.
        object fromZone = type == "Import" ? DBNull.Value : zoneId;

        using (var update = connection.CreateCommand())
        {
            update.Transaction = transaction;
            update.CommandText = "UPDATE warehouse_items SET quantity = $qty, last_movement_at = $movedAt WHERE id = $id";
            update.Parameters.AddWithValue("$qty", newQuantity);
            update.Parameters.AddWithValue("$movedAt", movedAt);
            update.Parameters.AddWithValue("$id", itemId);
            update.ExecuteNonQuery();
        }

        using (var insert = connection.CreateCommand())
        {
            insert.Transaction = transaction;
            insert.CommandText = """
                INSERT INTO goods_movements (id, item_id, from_zone_id, to_zone_id, quantity, movement_type, moved_by, moved_at, note)
                VALUES ($id, $itemId, $fromZone, $toZone, $qty, $type, $movedBy, $movedAt, $note)
                """;
            insert.Parameters.AddWithValue("$id", "gm-" + Guid.NewGuid().ToString("N"));
            insert.Parameters.AddWithValue("$itemId", itemId);
            insert.Parameters.AddWithValue("$fromZone", fromZone);
            insert.Parameters.AddWithValue("$toZone", zoneId);
            insert.Parameters.AddWithValue("$qty", quantity);
            insert.Parameters.AddWithValue("$type", type);
            insert.Parameters.AddWithValue("$movedBy", movedBy);
            insert.Parameters.AddWithValue("$movedAt", movedAt);
            insert.Parameters.AddWithValue("$note", note ?? $"{type} {quantity}");
            insert.ExecuteNonQuery();
        }

        transaction.Commit();
        return new MoveResult("ok", null, ReadItems(connection, itemId).FirstOrDefault());
    }

    private static string FirstUserId(SqliteConnection connection, SqliteTransaction transaction)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "SELECT id FROM users ORDER BY id LIMIT 1";
        return (string?)command.ExecuteScalar() ?? "user-001";
    }

    private static List<WarehouseItemDto> ReadItems(SqliteConnection connection, string? itemId = null)
    {
        const string baseSql = """
            SELECT w.id, w.io_id, w.io_code, w.bu, w.item_code, w.item_name, w.batch_code, w.category,
                   w.quantity, z.name AS zone, w.shelf, w.status, w.last_movement_at
            FROM warehouse_items w
            JOIN warehouse_zones z ON z.id = w.zone_id
            """;

        using var command = connection.CreateCommand();
        command.CommandText = itemId is null ? baseSql + " ORDER BY w.item_code" : baseSql + " WHERE w.id = $id";
        if (itemId is not null)
        {
            command.Parameters.AddWithValue("$id", itemId);
        }

        using var reader = command.ExecuteReader();
        var items = new List<WarehouseItemDto>();
        while (reader.Read())
        {
            items.Add(new WarehouseItemDto(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetString(5),
                reader.GetString(6),
                reader.GetString(7),
                reader.GetInt32(8),
                reader.GetString(9),
                reader.GetString(10),
                reader.GetString(11),
                TimeValue(reader.GetString(12))));
        }

        return items;
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
