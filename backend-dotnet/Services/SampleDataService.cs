using System.Text.Json;
using Microsoft.Data.Sqlite;
using SmartFactory.Api.Models;

namespace SmartFactory.Api.Services;

public sealed class SampleDataService
{
    private readonly string _dataSource;
    private readonly string? _fallbackReason;
    private readonly SampleData _data;
    private readonly Dictionary<string, FactoryArea> _areasById;
    private readonly Dictionary<string, User> _usersById;
    private readonly Dictionary<string, WarehouseZone> _zonesById;
    private readonly Dictionary<string, ProductionLineSource> _linesById;

    public SampleDataService(IWebHostEnvironment environment, IConfiguration configuration)
    {
        try
        {
            var connectionString = configuration.GetConnectionString("SmartFactoryDatabase") ?? "Data Source=database/smart_factory_demo.db";
            _data = LoadFromDatabase(environment.ContentRootPath, connectionString);
            _dataSource = "sqlite";
        }
        catch (Exception ex)
        {
            _fallbackReason = ex.Message;
            _data = LoadFromJson(environment.ContentRootPath);
            _dataSource = "json-fallback";
        }

        _areasById = _data.FactoryAreas.ToDictionary(item => item.Id);
        _usersById = _data.Users.ToDictionary(item => item.Id);
        _zonesById = _data.WarehouseZones.ToDictionary(item => item.Id);
        _linesById = _data.ProductionLines.ToDictionary(item => item.Id);
    }

    public object GetDataSourceStatus()
    {
        return new
        {
            source = _dataSource,
            fallbackReason = _fallbackReason
        };
    }

    public IReadOnlyList<KpiDto> GetKpis()
    {
        var lines = _data.ProductionLines;
        var totalTarget = lines.Sum(line => line.TargetOutput);
        var totalActual = lines.Sum(line => line.ActualOutput);
        var completion = totalTarget == 0 ? 0 : (int)Math.Round(totalActual * 100.0 / totalTarget);
        var activeLines = lines.Count(line => line.Status != "Stopped");
        var criticalAlerts = _data.SafetyAlerts.Count(alert => alert.Severity == "Critical" && alert.Status != "Resolved");

        return new[]
        {
            new KpiDto("Daily Output", totalActual.ToString("N0"), "Units completed today", "+6.4%", "good"),
            new KpiDto("Target Completion", $"{completion}%", "Against daily target", completion < 90 ? "Needs recovery" : "On track", completion < 90 ? "warning" : "good"),
            new KpiDto("Active Lines", $"{activeLines}/{lines.Count}", $"{lines.Count - activeLines} lines require attention", "Stable", "neutral"),
            new KpiDto("Safety Alerts", _data.SafetyAlerts.Count.ToString(), $"{criticalAlerts} critical alert open", criticalAlerts > 0 ? "Review now" : "Stable", criticalAlerts > 0 ? "danger" : "neutral")
        };
    }

    public IReadOnlyList<ProductionLineDto> GetProductionLines()
    {
        return _data.ProductionLines.Select(line => new ProductionLineDto(
            line.Id,
            line.Name,
            _areasById[line.AreaId].Name,
            line.Status,
            line.TargetOutput,
            line.ActualOutput,
            line.Efficiency,
            line.DefectRate,
            line.DowntimeMinutes,
            line.AssignedWorkers,
            line.Issue)).ToList();
    }

    public IReadOnlyList<SafetyAlertDto> GetSafetyAlerts()
    {
        return _data.SafetyAlerts.Select(alert => new SafetyAlertDto(
            alert.Id,
            alert.Title,
            alert.AlertType,
            alert.Severity,
            _areasById[alert.AreaId].Name,
            alert.Status,
            TimeValue(alert.DetectedAt),
            alert.Description)).ToList();
    }

    public IReadOnlyList<WarehouseItemDto> GetWarehouseItems()
    {
        return _data.WarehouseItems.Select(item => new WarehouseItemDto(
            item.Id,
            item.IoId,
            item.IoCode,
            item.Bu,
            item.ItemCode,
            item.ItemName,
            item.BatchCode,
            item.Category,
            item.Quantity,
            _zonesById[item.ZoneId].Name,
            item.Shelf,
            item.Status,
            TimeValue(item.LastMovementAt))).ToList();
    }

    public IReadOnlyList<ShiftPlanDto> GetShiftPlans()
    {
        return _data.ShiftPlans.Select(shift => new ShiftPlanDto(
            shift.Id,
            shift.ShiftName,
            _linesById[shift.LineId].Name,
            shift.RequiredWorkers,
            shift.AssignedWorkers,
            shift.OvertimeHours,
            shift.Status)).ToList();
    }

    public IReadOnlyList<FormRequestDto> GetFormRequests()
    {
        return _data.FormRequests.Select(form =>
        {
            var requester = _usersById[form.RequesterId];
            return new FormRequestDto(
                form.Id,
                form.FormType,
                requester.FullName,
                requester.Department,
                form.Status,
                TimeValue(form.SubmittedAt),
                form.Summary);
        }).ToList();
    }

    public IReadOnlyList<NotificationDto> GetNotifications()
    {
        return _data.Notifications.Select(notification => new NotificationDto(
            notification.Id,
            notification.Title,
            notification.NotificationType,
            notification.Severity,
            notification.Status,
            TimeValue(notification.CreatedAt))).ToList();
    }

    public IReadOnlyList<CameraEventDto> GetCameraEvents()
    {
        return _data.CameraEvents.Select(cameraEvent => new CameraEventDto(
            cameraEvent.Id,
            cameraEvent.CameraCode,
            _areasById[cameraEvent.AreaId].Name,
            cameraEvent.EventType,
            cameraEvent.Severity,
            cameraEvent.Confidence,
            TimeValue(cameraEvent.EventTime))).ToList();
    }

    public IReadOnlyList<string> GetRecommendationMessages()
    {
        return _data.AiRecommendations
            .Select(recommendation => $"{recommendation.Title}: {recommendation.ExpectedImpact}")
            .ToList();
    }

    private static JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private static SampleData LoadFromJson(string contentRootPath)
    {
        var dataFile = Path.GetFullPath(Path.Combine(contentRootPath, "database", "sample-data.json"));
        var json = File.ReadAllText(dataFile);
        return JsonSerializer.Deserialize<SampleData>(json, JsonOptions()) ?? new SampleData();
    }

    private static SampleData LoadFromDatabase(string contentRootPath, string configuredConnectionString)
    {
        var connectionString = NormalizeSqliteConnectionString(contentRootPath, configuredConnectionString);
        EnsureDatabaseCreated(contentRootPath, connectionString);

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        return new SampleData
        {
            Roles = ReadRows(connection, "SELECT id, name, description FROM roles", reader => new Role(GetText(reader, "id"), GetText(reader, "name"), GetText(reader, "description"))),
            Users = ReadRows(connection, "SELECT id, full_name, email, role_id, department, status, created_at FROM users", reader => new User(GetText(reader, "id"), GetText(reader, "full_name"), GetText(reader, "email"), GetText(reader, "role_id"), GetText(reader, "department"), GetText(reader, "status"), GetText(reader, "created_at"))),
            FactoryAreas = ReadRows(connection, "SELECT id, name, area_type, risk_level, description FROM factory_areas", reader => new FactoryArea(GetText(reader, "id"), GetText(reader, "name"), GetText(reader, "area_type"), GetText(reader, "risk_level"), GetText(reader, "description"))),
            ProductionLines = ReadRows(connection, "SELECT id, name, area_id, status, target_output, actual_output, efficiency, defect_rate, downtime_minutes, assigned_workers, issue FROM production_lines", reader => new ProductionLineSource(GetText(reader, "id"), GetText(reader, "name"), GetText(reader, "area_id"), GetText(reader, "status"), GetInt(reader, "target_output"), GetInt(reader, "actual_output"), GetInt(reader, "efficiency"), GetDouble(reader, "defect_rate"), GetInt(reader, "downtime_minutes"), GetInt(reader, "assigned_workers"), GetText(reader, "issue"))),
            ProductionRecords = ReadRows(connection, "SELECT id, line_id, record_time, target_output, actual_output, defect_count, downtime_minutes FROM production_records", reader => new ProductionRecord(GetText(reader, "id"), GetText(reader, "line_id"), GetText(reader, "record_time"), GetInt(reader, "target_output"), GetInt(reader, "actual_output"), GetInt(reader, "defect_count"), GetInt(reader, "downtime_minutes"))),
            Machines = ReadRows(connection, "SELECT id, machine_code, name, line_id, status, last_maintenance_at FROM machines", reader => new Machine(GetText(reader, "id"), GetText(reader, "machine_code"), GetText(reader, "name"), GetText(reader, "line_id"), GetText(reader, "status"), GetText(reader, "last_maintenance_at"))),
            MachineIssues = ReadRows(connection, "SELECT id, machine_id, line_id, severity, status, description, reported_by, reported_at FROM machine_issues", reader => new MachineIssue(GetText(reader, "id"), GetText(reader, "machine_id"), GetText(reader, "line_id"), GetText(reader, "severity"), GetText(reader, "status"), GetText(reader, "description"), GetText(reader, "reported_by"), GetText(reader, "reported_at"))),
            WarehouseZones = ReadRows(connection, "SELECT id, name, zone_type, capacity, current_usage, status FROM warehouse_zones", reader => new WarehouseZone(GetText(reader, "id"), GetText(reader, "name"), GetText(reader, "zone_type"), GetInt(reader, "capacity"), GetInt(reader, "current_usage"), GetText(reader, "status"))),
            WarehouseItems = ReadRows(connection, "SELECT id, io_id, io_code, bu, item_code, item_name, batch_code, category, quantity, unit, zone_id, shelf, status, last_movement_at FROM warehouse_items", reader => new WarehouseItemSource(GetText(reader, "id"), GetText(reader, "io_id"), GetText(reader, "io_code"), GetText(reader, "bu"), GetText(reader, "item_code"), GetText(reader, "item_name"), GetText(reader, "batch_code"), GetText(reader, "category"), GetInt(reader, "quantity"), GetText(reader, "unit"), GetText(reader, "zone_id"), GetText(reader, "shelf"), GetText(reader, "status"), GetText(reader, "last_movement_at"))),
            GoodsMovements = ReadRows(connection, "SELECT id, item_id, from_zone_id, to_zone_id, quantity, movement_type, moved_by, moved_at, note FROM goods_movements", reader => new GoodsMovement(GetText(reader, "id"), GetText(reader, "item_id"), GetNullableText(reader, "from_zone_id"), GetText(reader, "to_zone_id"), GetInt(reader, "quantity"), GetText(reader, "movement_type"), GetText(reader, "moved_by"), GetText(reader, "moved_at"), GetText(reader, "note"))),
            SafetyAlerts = ReadRows(connection, "SELECT id, title, alert_type, severity, area_id, status, detected_at, assigned_to, description, action_note FROM safety_alerts", reader => new SafetyAlertSource(GetText(reader, "id"), GetText(reader, "title"), GetText(reader, "alert_type"), GetText(reader, "severity"), GetText(reader, "area_id"), GetText(reader, "status"), GetText(reader, "detected_at"), GetText(reader, "assigned_to"), GetText(reader, "description"), GetNullableText(reader, "action_note"))),
            CameraEvents = ReadRows(connection, "SELECT id, camera_code, area_id, event_type, severity, confidence, event_time, related_alert_id FROM camera_events", reader => new CameraEventSource(GetText(reader, "id"), GetText(reader, "camera_code"), GetText(reader, "area_id"), GetText(reader, "event_type"), GetText(reader, "severity"), GetDouble(reader, "confidence"), GetText(reader, "event_time"), GetNullableText(reader, "related_alert_id"))),
            Employees = ReadRows(connection, "SELECT id, employee_code, full_name, department, skill_tags, availability_status, current_line_id FROM employees", reader => new Employee(GetText(reader, "id"), GetText(reader, "employee_code"), GetText(reader, "full_name"), GetText(reader, "department"), SplitSkillTags(GetText(reader, "skill_tags")), GetText(reader, "availability_status"), GetNullableText(reader, "current_line_id"))),
            ShiftPlans = ReadRows(connection, "SELECT id, shift_date, shift_name, line_id, required_workers, assigned_workers, overtime_hours, status FROM shift_plans", reader => new ShiftPlanSource(GetText(reader, "id"), GetText(reader, "shift_date"), GetText(reader, "shift_name"), GetText(reader, "line_id"), GetInt(reader, "required_workers"), GetInt(reader, "assigned_workers"), GetDouble(reader, "overtime_hours"), GetText(reader, "status"))),
            FormRequests = ReadRows(connection, "SELECT id, form_type, requester_id, approver_id, status, submitted_at, approved_at, summary, rejection_reason FROM form_requests", reader => new FormRequestSource(GetText(reader, "id"), GetText(reader, "form_type"), GetText(reader, "requester_id"), GetText(reader, "approver_id"), GetText(reader, "status"), GetText(reader, "submitted_at"), GetNullableText(reader, "approved_at"), GetText(reader, "summary"), GetNullableText(reader, "rejection_reason"))),
            Notifications = ReadRows(connection, "SELECT id, title, notification_type, severity, status, target_user_id, related_entity_type, related_entity_id, created_at FROM notifications", reader => new NotificationSource(GetText(reader, "id"), GetText(reader, "title"), GetText(reader, "notification_type"), GetText(reader, "severity"), GetText(reader, "status"), GetText(reader, "target_user_id"), GetText(reader, "related_entity_type"), GetText(reader, "related_entity_id"), GetText(reader, "created_at"))),
            AiRecommendations = ReadRows(connection, "SELECT id, module, title, reason, expected_impact, status, created_at, reviewed_by FROM ai_recommendations", reader => new AiRecommendation(GetText(reader, "id"), GetText(reader, "module"), GetText(reader, "title"), GetText(reader, "reason"), GetText(reader, "expected_impact"), GetText(reader, "status"), GetText(reader, "created_at"), GetNullableText(reader, "reviewed_by"))),
            Reports = ReadRows(connection, "SELECT id, report_type, title, period_start, period_end, summary, created_by, created_at FROM reports", reader => new Report(GetText(reader, "id"), GetText(reader, "report_type"), GetText(reader, "title"), GetText(reader, "period_start"), GetText(reader, "period_end"), GetText(reader, "summary"), GetText(reader, "created_by"), GetText(reader, "created_at")))
        };
    }

    private static string NormalizeSqliteConnectionString(string contentRootPath, string configuredConnectionString)
    {
        var builder = new SqliteConnectionStringBuilder(configuredConnectionString);
        if (string.IsNullOrWhiteSpace(builder.DataSource))
        {
            builder.DataSource = Path.Combine(contentRootPath, "database", "smart_factory_demo.db");
        }
        else if (!Path.IsPathRooted(builder.DataSource) && builder.DataSource != ":memory:")
        {
            builder.DataSource = Path.GetFullPath(Path.Combine(contentRootPath, builder.DataSource));
        }

        return builder.ToString();
    }

    private static void EnsureDatabaseCreated(string contentRootPath, string connectionString)
    {
        var builder = new SqliteConnectionStringBuilder(connectionString);
        if (!string.IsNullOrWhiteSpace(builder.DataSource) && builder.DataSource != ":memory:")
        {
            var databaseDirectory = Path.GetDirectoryName(builder.DataSource);
            if (!string.IsNullOrWhiteSpace(databaseDirectory))
            {
                Directory.CreateDirectory(databaseDirectory);
            }
        }

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        if (TableHasRows(connection, "roles"))
        {
            return;
        }

        ExecuteSqlFile(connection, Path.Combine(contentRootPath, "database", "schema.sql"));
        ExecuteSqlFile(connection, Path.Combine(contentRootPath, "database", "seed.sql"));
    }

    private static bool TableHasRows(SqliteConnection connection, string tableName)
    {
        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT EXISTS (SELECT 1 FROM {tableName} LIMIT 1)";
            return Convert.ToInt32(command.ExecuteScalar()) == 1;
        }
        catch (SqliteException)
        {
            return false;
        }
    }

    private static void ExecuteSqlFile(SqliteConnection connection, string filePath)
    {
        using var command = connection.CreateCommand();
        command.CommandText = File.ReadAllText(filePath);
        command.ExecuteNonQuery();
    }

    private static List<T> ReadRows<T>(SqliteConnection connection, string sql, Func<SqliteDataReader, T> map)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        using var reader = command.ExecuteReader();

        var rows = new List<T>();
        while (reader.Read())
        {
            rows.Add(map(reader));
        }

        return rows;
    }

    private static string GetText(SqliteDataReader reader, string columnName)
    {
        return reader.GetString(reader.GetOrdinal(columnName));
    }

    private static string? GetNullableText(SqliteDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }

    private static int GetInt(SqliteDataReader reader, string columnName)
    {
        return reader.GetInt32(reader.GetOrdinal(columnName));
    }

    private static double GetDouble(SqliteDataReader reader, string columnName)
    {
        return reader.GetDouble(reader.GetOrdinal(columnName));
    }

    private static List<string> SplitSkillTags(string value)
    {
        return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
    }

    private static string TimeValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Contains('T') && value.Length >= 16 ? value.Substring(11, 5) : value;
    }
}