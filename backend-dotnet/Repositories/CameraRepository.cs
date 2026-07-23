using Microsoft.EntityFrameworkCore;
using SmartFactory.Api.Data;
using SmartFactory.Api.Models;
using SmartFactory.Api.Models.Database;
using SmartFactory.Api.Services;

namespace SmartFactory.Api.Repositories;

/// <summary>
/// EF Core access for the AI camera module. Ingests a (simulated) camera detection
/// and, when it is severe and confident enough, auto-raises a linked safety_alert
/// (two-way linked via safety_alert_camera_events + related_alert_id) and a safety
/// notification. The detection rules (confidence threshold, allowed/alert severities)
/// come from the app_settings table, and camera→area mapping from the cameras table,
/// so they can be changed in the database without a code change.
/// </summary>
public sealed class CameraRepository(SmartFactoryDbContext context, AppSettingsService settings)
{
    public sealed record DetectResult(string Status, string? Error, CameraEventDto? Event, bool AlertRaised, string? AlertId);

    // Fallback rules, used only when the corresponding app_settings row is missing,
    // so detection keeps working even if the configuration table was not seeded.
    private const double DefaultAutoAlertConfidence = 0.80;
    private static readonly string[] DefaultAllowedSeverities = ["Low", "Medium", "High", "Critical"];
    private static readonly string[] DefaultAlertSeverities = ["High", "Critical"];

    public bool IsAvailable() => context.Database.CanConnect();

    public IReadOnlyList<CameraDto> GetCameras()
    {
        return context.Cameras
            .OrderBy(camera => camera.CameraCode)
            .Select(camera => new CameraDto(camera.CameraCode, camera.Name, camera.Status))
            .ToList();
    }

    public IReadOnlyList<CameraEventDto> GetEvents()
    {
        return (from ev in context.CameraEvents
                join area in context.FactoryAreas on ev.AreaId equals area.Id
                orderby ev.EventTime descending
                select new { ev, Location = area.Name })
            .AsEnumerable()
            .Select(row => ToDto(row.ev, row.Location))
            .ToList();
    }

    public DetectResult Detect(string? cameraCode, string? eventType, string? severity, double? confidence)
    {
        var camera = (cameraCode ?? string.Empty).Trim().ToUpperInvariant();
        var type = (eventType ?? string.Empty).Trim();
        var level = (severity ?? string.Empty).Trim();
        var score = confidence ?? 0d;
        var config = LoadConfig();

        if (string.IsNullOrEmpty(camera))
        {
            return new DetectResult("invalid", "cameraCode is required.", null, false, null);
        }

        if (string.IsNullOrEmpty(type))
        {
            return new DetectResult("invalid", "eventType is required.", null, false, null);
        }

        if (!config.AllowedSeverities.Contains(level))
        {
            return new DetectResult("invalid", $"severity must be one of: {string.Join(", ", config.AllowedSeverities)}.", null, false, null);
        }

        if (score < 0d || score > 1d)
        {
            return new DetectResult("invalid", "confidence must be between 0 and 1.", null, false, null);
        }

        using var transaction = context.Database.BeginTransaction();

        var areaId = ResolveAreaId(camera);
        var detectedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
        var eventId = "cam-event-" + Guid.NewGuid().ToString("N");

        var cameraEvent = new CameraEventEntity
        {
            Id = eventId,
            CameraCode = camera,
            AreaId = areaId,
            EventType = type,
            Severity = level,
            Confidence = score,
            EventTime = detectedAt,
            RelatedAlertId = null
        };
        context.CameraEvents.Add(cameraEvent);

        // Escalate to a safety alert only for serious, confident detections; otherwise
        // the event is just logged (so low-severity noise does not spam the safety team).
        string? alertId = null;
        var raiseAlert = config.AlertSeverities.Contains(level) && score >= config.AutoAlertConfidence;
        if (raiseAlert)
        {
            alertId = RaiseAlert(cameraEvent, camera, areaId, type, level, score, detectedAt);
        }

        context.SaveChanges();
        transaction.Commit();

        var dto = ToDto(cameraEvent, AreaName(areaId));
        return new DetectResult("ok", null, dto, raiseAlert, alertId);
    }

    // Creates a safety alert from the detection, links it two-way to the camera event,
    // and notifies the safety officer. Returns the new alert id.
    private string RaiseAlert(CameraEventEntity cameraEvent, string camera, string areaId, string type, string severity, double score, string detectedAt)
    {
        var alertId = "safe-auto-" + Guid.NewGuid().ToString("N");
        var safetyUser = SafetyUserId();
        var location = AreaName(areaId);
        var percent = (int)Math.Round(score * 100);

        context.SafetyAlerts.Add(new SafetyAlertEntity
        {
            Id = alertId,
            Title = $"{type} detected by {camera}",
            AlertType = "AI Camera",
            Severity = severity,
            AreaId = areaId,
            Status = "New",
            DetectedAt = detectedAt,
            AssignedTo = safetyUser,
            Description = $"Auto-raised from {camera} detection in {location} at {percent}% confidence.",
            ActionNote = null
        });

        cameraEvent.RelatedAlertId = alertId;

        context.SafetyAlertCameraEvents.Add(new SafetyAlertCameraEventEntity
        {
            AlertId = alertId,
            CameraEventId = cameraEvent.Id,
            RelationType = "Primary Evidence",
            LinkedAt = detectedAt
        });

        context.Notifications.Add(new NotificationEntity
        {
            Id = "noti-cam-" + Guid.NewGuid().ToString("N"),
            Title = $"Safety alert: {type} at {location}",
            NotificationType = "Safety",
            Severity = severity,
            Status = "Unread",
            TargetUserId = safetyUser,
            RelatedEntityType = "SafetyAlert",
            RelatedEntityId = alertId,
            CreatedAt = detectedAt
        });

        return alertId;
    }

    // --- Configuration & lookups (from the database) -----------------------

    private sealed record CameraConfig(double AutoAlertConfidence, string[] AllowedSeverities, string[] AlertSeverities);

    // Loads the camera detection rules from app_settings, falling back to the
    // built-in defaults for any setting that is missing.
    private CameraConfig LoadConfig() => new(
        settings.GetDouble("camera.auto_alert_confidence", DefaultAutoAlertConfidence),
        settings.GetList("camera.allowed_severities", DefaultAllowedSeverities),
        settings.GetList("camera.alert_severities", DefaultAlertSeverities));

    // Looks up the area the camera watches (from the cameras table), falling back to
    // any existing area so an unknown camera code still records a valid event.
    private string ResolveAreaId(string camera)
    {
        var areaId = context.Cameras.Where(item => item.CameraCode == camera).Select(item => item.AreaId).FirstOrDefault();
        if (areaId is not null && context.FactoryAreas.Any(area => area.Id == areaId))
        {
            return areaId;
        }

        return context.FactoryAreas.OrderBy(area => area.Id).Select(area => area.Id).FirstOrDefault() ?? "area-robot";
    }

    private string AreaName(string areaId) =>
        context.FactoryAreas.Where(area => area.Id == areaId).Select(area => area.Name).FirstOrDefault() ?? areaId;

    // Prefer a Safety-department user as the alert owner / notification target, else any user.
    private string SafetyUserId()
    {
        return context.Users.Where(user => user.Department == "Safety").OrderBy(user => user.Id).Select(user => user.Id).FirstOrDefault()
            ?? context.Users.OrderBy(user => user.Id).Select(user => user.Id).FirstOrDefault()
            ?? "user-004";
    }

    private static CameraEventDto ToDto(CameraEventEntity ev, string location) => new(
        ev.Id,
        ev.CameraCode,
        location,
        ev.EventType,
        ev.Severity,
        ev.Confidence,
        TimeValue(ev.EventTime),
        ev.RelatedAlertId);

    private static string TimeValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Contains('T') && value.Length >= 16 ? value.Substring(11, 5) : value;
    }
}
