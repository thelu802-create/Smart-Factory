using Microsoft.Data.Sqlite;
using SmartFactory.Api.Data;
using SmartFactory.Api.Models;

namespace SmartFactory.Api.Repositories;

/// <summary>
/// Live SQLite access for the warehouse module. Records stock movements as a
/// multi-table transaction: inserts a goods_movements row, adjusts the item's
/// quantity/zone, recomputes the item status and the affected zones' usage/status,
/// all atomically and with validation.
/// </summary>
public sealed class WarehouseRepository(DbConnectionFactory connectionFactory)
{
    /// <summary>Result of a move: Status is "ok", "not_found", or "invalid".</summary>
    public sealed record MoveResult(string Status, string? Error, WarehouseItemDto? Item);

    // Recompute thresholds. LowStock chosen so it reproduces the seed statuses exactly.
    private const int LowStockThreshold = 100;
    private const double WarningRatio = 0.95;
    private const double NearCapacityRatio = 0.85;

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

    public IReadOnlyList<WarehouseZoneDto> GetZones()
    {
        using var connection = connectionFactory.CreateOpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, name, zone_type, capacity, current_usage, status FROM warehouse_zones ORDER BY name";
        using var reader = command.ExecuteReader();
        var zones = new List<WarehouseZoneDto>();
        while (reader.Read())
        {
            zones.Add(new WarehouseZoneDto(reader.GetString(0), reader.GetString(1), reader.GetString(2),
                reader.GetInt32(3), reader.GetInt32(4), reader.GetString(5)));
        }

        return zones;
    }

    public IReadOnlyList<GoodsMovementDto> GetMovements(string itemId)
    {
        using var connection = connectionFactory.CreateOpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT m.id, m.movement_type, m.quantity, zf.name, zt.name, m.moved_at, m.note
            FROM goods_movements m
            LEFT JOIN warehouse_zones zf ON zf.id = m.from_zone_id
            JOIN warehouse_zones zt ON zt.id = m.to_zone_id
            WHERE m.item_id = $id
            ORDER BY m.moved_at DESC
            """;
        command.Parameters.AddWithValue("$id", itemId);
        using var reader = command.ExecuteReader();
        var movements = new List<GoodsMovementDto>();
        while (reader.Read())
        {
            movements.Add(new GoodsMovementDto(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                reader.GetString(4),
                TimeValue(reader.GetString(5)),
                reader.GetString(6)));
        }

        return movements;
    }

    public MoveResult Move(string itemId, string? movementType, int quantity, string? toZoneId, string? note)
    {
        var type = (movementType ?? string.Empty).Trim();
        if (type != "Import" && type != "Export" && type != "Transfer")
        {
            return new MoveResult("invalid", "movementType must be 'Import', 'Export', or 'Transfer'.", null);
        }

        using var connection = connectionFactory.CreateOpenConnection();
        using var transaction = connection.BeginTransaction();

        // Read current item state.
        int currentQuantity;
        string currentZoneId;
        string category;
        using (var read = connection.CreateCommand())
        {
            read.Transaction = transaction;
            read.CommandText = "SELECT quantity, zone_id, category FROM warehouse_items WHERE id = $id";
            read.Parameters.AddWithValue("$id", itemId);
            using var reader = read.ExecuteReader();
            if (!reader.Read())
            {
                return new MoveResult("not_found", "Warehouse item not found.", null);
            }

            currentQuantity = reader.GetInt32(0);
            currentZoneId = reader.GetString(1);
            category = reader.GetString(2);
        }

        int newQuantity = currentQuantity;
        string effectiveZoneId = currentZoneId;
        int movementQuantity;
        object fromZone;
        string toZone;

        if (type == "Import")
        {
            if (quantity <= 0)
            {
                return new MoveResult("invalid", "quantity must be greater than 0.", null);
            }

            newQuantity = currentQuantity + quantity;
            movementQuantity = quantity;
            fromZone = DBNull.Value;
            toZone = currentZoneId;
            AdjustZone(connection, transaction, currentZoneId, quantity);
        }
        else if (type == "Export")
        {
            if (quantity <= 0)
            {
                return new MoveResult("invalid", "quantity must be greater than 0.", null);
            }

            if (quantity > currentQuantity)
            {
                return new MoveResult("invalid", $"Export quantity ({quantity}) exceeds available stock ({currentQuantity}).", null);
            }

            newQuantity = currentQuantity - quantity;
            movementQuantity = quantity;
            fromZone = currentZoneId;
            toZone = currentZoneId;
            AdjustZone(connection, transaction, currentZoneId, -quantity);
        }
        else // Transfer: relocate the whole item to another zone.
        {
            var target = (toZoneId ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(target))
            {
                return new MoveResult("invalid", "toZoneId is required for a Transfer.", null);
            }

            if (target == currentZoneId)
            {
                return new MoveResult("invalid", "Transfer target zone must differ from the current zone.", null);
            }

            if (!ZoneExists(connection, transaction, target))
            {
                return new MoveResult("invalid", "Target zone does not exist.", null);
            }

            effectiveZoneId = target;
            movementQuantity = currentQuantity;
            fromZone = currentZoneId;
            toZone = target;
            AdjustZone(connection, transaction, currentZoneId, -currentQuantity);
            AdjustZone(connection, transaction, target, currentQuantity);
        }

        var movedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
        var newStatus = ComputeItemStatus(category, ZoneType(connection, transaction, effectiveZoneId), newQuantity);

        using (var update = connection.CreateCommand())
        {
            update.Transaction = transaction;
            update.CommandText = "UPDATE warehouse_items SET quantity = $qty, zone_id = $zone, status = $status, last_movement_at = $movedAt WHERE id = $id";
            update.Parameters.AddWithValue("$qty", newQuantity);
            update.Parameters.AddWithValue("$zone", effectiveZoneId);
            update.Parameters.AddWithValue("$status", newStatus);
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
            insert.Parameters.AddWithValue("$toZone", toZone);
            insert.Parameters.AddWithValue("$qty", movementQuantity);
            insert.Parameters.AddWithValue("$type", type);
            insert.Parameters.AddWithValue("$movedBy", FirstUserId(connection, transaction));
            insert.Parameters.AddWithValue("$movedAt", movedAt);
            insert.Parameters.AddWithValue("$note", note ?? $"{type} {movementQuantity}");
            insert.ExecuteNonQuery();
        }

        transaction.Commit();
        return new MoveResult("ok", null, ReadItems(connection, itemId).FirstOrDefault());
    }

    // --- Recompute helpers -------------------------------------------------

    // Item status: Low Stock below the threshold, otherwise Correct when the zone
    // type suits the category, else Wrong Zone. Reproduces the seed statuses exactly.
    private static string ComputeItemStatus(string category, string zoneType, int quantity)
    {
        if (quantity <= LowStockThreshold)
        {
            return "Low Stock";
        }

        return ZoneSuitsCategory(category, zoneType) ? "Correct" : "Wrong Zone";
    }

    private static bool ZoneSuitsCategory(string category, string zoneType)
    {
        if (zoneType == "Temperature Sensitive")
        {
            return true; // special-purpose zone accepts any category
        }

        return category switch
        {
            "Raw Material" or "Component" => zoneType == "Raw Material",
            "Finished Goods" => zoneType == "Finished Goods",
            "Packaging" => zoneType == "Packaging",
            _ => false
        };
    }

    // Applies a usage delta to a zone (clamped at 0) and recomputes its status.
    private static void AdjustZone(SqliteConnection connection, SqliteTransaction transaction, string zoneId, int delta)
    {
        int usage, capacity;
        using (var read = connection.CreateCommand())
        {
            read.Transaction = transaction;
            read.CommandText = "SELECT current_usage, capacity FROM warehouse_zones WHERE id = $id";
            read.Parameters.AddWithValue("$id", zoneId);
            using var reader = read.ExecuteReader();
            if (!reader.Read())
            {
                return;
            }

            usage = reader.GetInt32(0);
            capacity = reader.GetInt32(1);
        }

        var newUsage = Math.Max(0, usage + delta);
        using var update = connection.CreateCommand();
        update.Transaction = transaction;
        update.CommandText = "UPDATE warehouse_zones SET current_usage = $usage, status = $status WHERE id = $id";
        update.Parameters.AddWithValue("$usage", newUsage);
        update.Parameters.AddWithValue("$status", ComputeZoneStatus(newUsage, capacity));
        update.Parameters.AddWithValue("$id", zoneId);
        update.ExecuteNonQuery();
    }

    private static string ComputeZoneStatus(int usage, int capacity)
    {
        var ratio = capacity <= 0 ? 0 : usage / (double)capacity;
        if (ratio >= WarningRatio) return "Warning";
        if (ratio >= NearCapacityRatio) return "Near Capacity";
        return "Available";
    }

    // --- Small query helpers ----------------------------------------------

    private static bool ZoneExists(SqliteConnection connection, SqliteTransaction transaction, string zoneId)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "SELECT 1 FROM warehouse_zones WHERE id = $id";
        command.Parameters.AddWithValue("$id", zoneId);
        return command.ExecuteScalar() is not null;
    }

    private static string ZoneType(SqliteConnection connection, SqliteTransaction transaction, string zoneId)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "SELECT zone_type FROM warehouse_zones WHERE id = $id";
        command.Parameters.AddWithValue("$id", zoneId);
        return (string?)command.ExecuteScalar() ?? string.Empty;
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
                reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3),
                reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7),
                reader.GetInt32(8), reader.GetString(9), reader.GetString(10), reader.GetString(11),
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
