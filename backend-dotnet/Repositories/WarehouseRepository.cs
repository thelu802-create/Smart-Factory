using Microsoft.EntityFrameworkCore;
using SmartFactory.Api.Models;
using SmartFactory.Api.Models.Database;
using SmartFactory.Api.Data;
using SmartFactory.Api.Services;

namespace SmartFactory.Api.Repositories;

/// <summary>
/// EF Core access for the warehouse module. Records stock movements as a
/// transaction: inserts a goods_movements row, adjusts the item's quantity/zone,
/// recomputes the item status and the affected zones' usage/status, all atomically
/// and with validation. Auto-raises a notification on a Low Stock / Wrong Zone
/// transition.
/// </summary>
public sealed class WarehouseRepository(SmartFactoryDbContext context, AppSettingsService settings)
{
    /// <summary>Result of a move: Status is "ok", "not_found", or "invalid".</summary>
    public sealed record MoveResult(string Status, string? Error, WarehouseItemDto? Item);

    // Fallback thresholds, used only when the matching app_settings row is missing.
    private const int DefaultLowStockThreshold = 100;
    private const double DefaultWarningRatio = 0.95;
    private const double DefaultNearCapacityRatio = 0.85;

    public bool IsAvailable() => context.Database.CanConnect();

    public IReadOnlyList<WarehouseItemDto> GetItems()
    {
        return (from item in context.WarehouseItems
                join zone in context.WarehouseZones on item.ZoneId equals zone.Id
                orderby item.ItemCode
                select new { item, ZoneName = zone.Name })
            .AsEnumerable()
            .Select(row => ToDto(row.item, row.ZoneName))
            .ToList();
    }

    public WarehouseItemDto? GetItem(string itemId)
    {
        var row = (from item in context.WarehouseItems
                   join zone in context.WarehouseZones on item.ZoneId equals zone.Id
                   where item.Id == itemId
                   select new { item, ZoneName = zone.Name })
            .FirstOrDefault();
        return row is null ? null : ToDto(row.item, row.ZoneName);
    }

    public IReadOnlyList<WarehouseZoneDto> GetZones()
    {
        return context.WarehouseZones
            .OrderBy(zone => zone.Name)
            .Select(zone => new WarehouseZoneDto(zone.Id, zone.Name, zone.ZoneType, zone.Capacity, zone.CurrentUsage, zone.Status))
            .ToList();
    }

    public IReadOnlyList<GoodsMovementDto> GetMovements(string itemId)
    {
        return (from movement in context.GoodsMovements
                where movement.ItemId == itemId
                join zt in context.WarehouseZones on movement.ToZoneId equals zt.Id
                join zf in context.WarehouseZones on movement.FromZoneId equals zf.Id into fromJoin
                from zf in fromJoin.DefaultIfEmpty()
                orderby movement.MovedAt descending
                select new { movement, ToName = zt.Name, FromName = zf != null ? zf.Name : null })
            .AsEnumerable()
            .Select(row => new GoodsMovementDto(
                row.movement.Id,
                row.movement.MovementType,
                row.movement.Quantity,
                row.FromName,
                row.ToName,
                TimeValue(row.movement.MovedAt),
                row.movement.Note))
            .ToList();
    }

    /// <summary>
    /// Records a stock movement in its own transaction. See <see cref="ApplyMove"/> for
    /// the version that participates in a caller-managed transaction.
    /// </summary>
    public MoveResult Move(string itemId, string? movementType, int quantity, string? toZoneId, string? note)
    {
        using var transaction = context.Database.BeginTransaction();
        var result = ApplyMove(itemId, movementType, quantity, toZoneId, note);
        if (result.Status != "ok")
        {
            return result; // transaction rolls back on dispose
        }

        context.SaveChanges();
        transaction.Commit();
        return new MoveResult("ok", null, GetItem(itemId));
    }

    /// <summary>
    /// Validates and applies a stock movement to the tracked entities WITHOUT opening a
    /// transaction or calling SaveChanges — the caller owns the transaction and commit.
    /// Lets the forms module deduct borrowed stock inside the same transaction as the
    /// approval. Returns Item = null on success (query it after the caller commits).
    /// </summary>
    public MoveResult ApplyMove(string itemId, string? movementType, int quantity, string? toZoneId, string? note)
    {
        var type = (movementType ?? string.Empty).Trim();
        if (type != "Import" && type != "Export" && type != "Transfer")
        {
            return new MoveResult("invalid", "movementType must be 'Import', 'Export', or 'Transfer'.", null);
        }

        // Recompute thresholds come from app_settings (fall back to the defaults).
        var lowStockThreshold = settings.GetInt("warehouse.low_stock_threshold", DefaultLowStockThreshold);
        var warningRatio = settings.GetDouble("warehouse.zone_warning_ratio", DefaultWarningRatio);
        var nearCapacityRatio = settings.GetDouble("warehouse.zone_near_capacity_ratio", DefaultNearCapacityRatio);

        var item = context.WarehouseItems.FirstOrDefault(row => row.Id == itemId);
        if (item is null)
        {
            return new MoveResult("not_found", "Warehouse item not found.", null);
        }

        var currentQuantity = item.Quantity;
        var currentZoneId = item.ZoneId;
        var oldStatus = item.Status;

        int newQuantity;
        var effectiveZoneId = currentZoneId;
        int movementQuantity;
        string? fromZone;
        string toZone;

        if (type == "Import")
        {
            if (quantity <= 0)
            {
                return new MoveResult("invalid", "quantity must be greater than 0.", null);
            }

            newQuantity = currentQuantity + quantity;
            movementQuantity = quantity;
            fromZone = null;
            toZone = currentZoneId;
            AdjustZone(currentZoneId, quantity, warningRatio, nearCapacityRatio);
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
            AdjustZone(currentZoneId, -quantity, warningRatio, nearCapacityRatio);
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

            if (!context.WarehouseZones.Any(zone => zone.Id == target))
            {
                return new MoveResult("invalid", "Target zone does not exist.", null);
            }

            newQuantity = currentQuantity;
            effectiveZoneId = target;
            movementQuantity = currentQuantity;
            fromZone = currentZoneId;
            toZone = target;
            AdjustZone(currentZoneId, -currentQuantity, warningRatio, nearCapacityRatio);
            AdjustZone(target, currentQuantity, warningRatio, nearCapacityRatio);
        }

        var movedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
        var newStatus = ComputeItemStatus(item.Category, ZoneType(effectiveZoneId), newQuantity, lowStockThreshold);

        item.Quantity = newQuantity;
        item.ZoneId = effectiveZoneId;
        item.Status = newStatus;
        item.LastMovementAt = movedAt;

        context.GoodsMovements.Add(new GoodsMovementEntity
        {
            Id = "gm-" + Guid.NewGuid().ToString("N"),
            ItemId = itemId,
            FromZoneId = fromZone,
            ToZoneId = toZone,
            Quantity = movementQuantity,
            MovementType = type,
            MovedBy = FirstUserId(),
            MovedAt = movedAt,
            Note = note ?? $"{type} {movementQuantity}"
        });

        // Raise a warehouse notification when a movement pushes the item into a problem
        // status it was not already in (avoids repeat alerts on every move).
        if (newStatus != oldStatus && (newStatus == "Low Stock" || newStatus == "Wrong Zone"))
        {
            RaiseStatusNotification(itemId, item.ItemCode, newStatus, movedAt);
        }

        return new MoveResult("ok", null, null);
    }

    // --- Recompute helpers -------------------------------------------------

    private static string ComputeItemStatus(string category, string zoneType, int quantity, int lowStockThreshold)
    {
        if (quantity <= lowStockThreshold)
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

    // Applies a usage delta to a tracked zone (clamped at 0) and recomputes its status.
    private void AdjustZone(string zoneId, int delta, double warningRatio, double nearCapacityRatio)
    {
        var zone = context.WarehouseZones.FirstOrDefault(item => item.Id == zoneId);
        if (zone is null)
        {
            return;
        }

        zone.CurrentUsage = Math.Max(0, zone.CurrentUsage + delta);
        zone.Status = ComputeZoneStatus(zone.CurrentUsage, zone.Capacity, warningRatio, nearCapacityRatio);
    }

    private static string ComputeZoneStatus(int usage, int capacity, double warningRatio, double nearCapacityRatio)
    {
        var ratio = capacity <= 0 ? 0 : usage / (double)capacity;
        if (ratio >= warningRatio) return "Warning";
        if (ratio >= nearCapacityRatio) return "Near Capacity";
        return "Available";
    }

    private string ZoneType(string zoneId) =>
        context.WarehouseZones.Where(zone => zone.Id == zoneId).Select(zone => zone.ZoneType).FirstOrDefault() ?? string.Empty;

    private string FirstUserId() =>
        context.Users.OrderBy(user => user.Id).Select(user => user.Id).FirstOrDefault() ?? "user-001";

    private string WarehouseUserId() =>
        context.Users.Where(user => user.Department == "Warehouse").OrderBy(user => user.Id).Select(user => user.Id).FirstOrDefault()
            ?? FirstUserId();

    private void RaiseStatusNotification(string itemId, string itemCode, string status, string createdAt)
    {
        var (title, severity) = status == "Low Stock"
            ? ($"Low stock: {itemCode}", "Medium")
            : ($"Wrong placement: {itemCode}", "High");

        context.Notifications.Add(new NotificationEntity
        {
            Id = "noti-wh-" + Guid.NewGuid().ToString("N"),
            Title = title,
            NotificationType = "Warehouse",
            Severity = severity,
            Status = "Unread",
            TargetUserId = WarehouseUserId(),
            RelatedEntityType = "WarehouseItem",
            RelatedEntityId = itemId,
            CreatedAt = createdAt
        });
    }

    private static WarehouseItemDto ToDto(WarehouseItemEntity item, string zoneName) => new(
        item.Id,
        item.IoId,
        item.IoCode,
        item.Bu,
        item.ItemCode,
        item.ItemName,
        item.BatchCode,
        item.Category,
        item.Quantity,
        zoneName,
        item.Shelf,
        item.Status,
        TimeValue(item.LastMovementAt));

    private static string TimeValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Contains('T') && value.Length >= 16 ? value.Substring(11, 5) : value;
    }
}
