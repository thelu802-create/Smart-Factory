using SmartFactory.Api.Data;
using SmartFactory.Api.Models;

namespace SmartFactory.Api.Services;

/// <summary>
/// Read/aggregation service for dashboard and module list views. Now backed by EF
/// Core (SmartFactory SQL Server database) — every call reads live data, so the
/// snapshot/JSON-fallback behaviour of the old SQLite implementation is gone.
/// Method names and returned DTOs are unchanged so the controllers stay identical.
/// </summary>
public sealed class SampleDataService(SmartFactoryDbContext context, AppSettingsService settings)
{
    public object GetDataSourceStatus()
    {
        return new { source = "sqlserver", fallbackReason = (string?)null };
    }

    public IReadOnlyList<KpiDto> GetKpis()
    {
        var lines = context.ProductionLines.ToList();
        var totalTarget = lines.Sum(line => line.TargetOutput);
        var totalActual = lines.Sum(line => line.ActualOutput);
        var completion = totalTarget == 0 ? 0 : (int)Math.Round(totalActual * 100.0 / totalTarget);
        var activeLines = lines.Count(line => line.Status != "Stopped");
        var alerts = context.SafetyAlerts.ToList();
        var criticalAlerts = alerts.Count(alert => alert.Severity == "Critical" && alert.Status != "Resolved");
        var targetWarning = settings.GetInt("dashboard.target_completion_warning", 90);

        return new[]
        {
            new KpiDto("Daily Output", totalActual.ToString("N0"), "Units completed today", "+6.4%", "good"),
            new KpiDto("Target Completion", $"{completion}%", "Against daily target", completion < targetWarning ? "Needs recovery" : "On track", completion < targetWarning ? "warning" : "good"),
            new KpiDto("Active Lines", $"{activeLines}/{lines.Count}", $"{lines.Count - activeLines} lines require attention", "Stable", "neutral"),
            new KpiDto("Safety Alerts", alerts.Count.ToString(), $"{criticalAlerts} critical alert open", criticalAlerts > 0 ? "Review now" : "Stable", criticalAlerts > 0 ? "danger" : "neutral")
        };
    }

    public IReadOnlyList<ProductionLineDto> GetProductionLines()
    {
        return (from line in context.ProductionLines
                join area in context.FactoryAreas on line.AreaId equals area.Id
                orderby line.Id
                select new ProductionLineDto(
                    line.Id, line.Name, area.Name, line.Status, line.TargetOutput, line.ActualOutput,
                    line.Efficiency, line.DefectRate, line.DowntimeMinutes, line.AssignedWorkers, line.Issue))
            .ToList();
    }

    public IReadOnlyList<SafetyAlertDto> GetSafetyAlerts()
    {
        return (from alert in context.SafetyAlerts
                join area in context.FactoryAreas on alert.AreaId equals area.Id
                orderby alert.Id
                select new { alert, area.Name })
            .AsEnumerable()
            .Select(row => new SafetyAlertDto(
                row.alert.Id, row.alert.Title, row.alert.AlertType, row.alert.Severity, row.Name,
                row.alert.Status, TimeValue(row.alert.DetectedAt), row.alert.Description))
            .ToList();
    }

    public IReadOnlyList<WarehouseItemDto> GetWarehouseItems()
    {
        return (from item in context.WarehouseItems
                join zone in context.WarehouseZones on item.ZoneId equals zone.Id
                orderby item.ItemCode
                select new { item, ZoneName = zone.Name })
            .AsEnumerable()
            .Select(row => new WarehouseItemDto(
                row.item.Id, row.item.IoId, row.item.IoCode, row.item.Bu, row.item.ItemCode, row.item.ItemName,
                row.item.BatchCode, row.item.Category, row.item.Quantity, row.ZoneName, row.item.Shelf,
                row.item.Status, TimeValue(row.item.LastMovementAt)))
            .ToList();
    }

    public IReadOnlyList<ShiftPlanDto> GetShiftPlans()
    {
        return (from shift in context.ShiftPlans
                join line in context.ProductionLines on shift.LineId equals line.Id
                orderby shift.Id
                select new ShiftPlanDto(
                    shift.Id, shift.ShiftName, line.Name, shift.RequiredWorkers, shift.AssignedWorkers,
                    shift.OvertimeHours, shift.Status))
            .ToList();
    }

    public IReadOnlyList<FormRequestDto> GetFormRequests()
    {
        return (from form in context.FormRequests
                join user in context.Users on form.RequesterId equals user.Id
                orderby form.Id
                select new { form, user.FullName, user.Department })
            .AsEnumerable()
            .Select(row => new FormRequestDto(
                row.form.Id, row.form.FormType, row.FullName, row.Department, row.form.Status,
                TimeValue(row.form.SubmittedAt), row.form.Summary))
            .ToList();
    }

    // Reduces an ISO timestamp to "HH:mm" for display, matching the original behaviour.
    private static string TimeValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Contains('T') && value.Length >= 16 ? value.Substring(11, 5) : value;
    }
}
