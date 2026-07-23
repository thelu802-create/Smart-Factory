namespace SmartFactory.Api.Models;

// Response DTOs returned by the read service and repositories. The domain model
// itself now lives in Models/Database (EF Core entities); these are the shapes the
// frontend consumes, kept separate so the API surface is decoupled from the schema.

public sealed record KpiDto(string Label, string Value, string Detail, string Trend, string Tone);
public sealed record ProductionLineDto(string Id, string Name, string Area, string Status, int TargetOutput, int ActualOutput, int Efficiency, double DefectRate, int DowntimeMinutes, int AssignedWorkers, string Issue);
public sealed record SafetyAlertDto(string Id, string Title, string Type, string Severity, string Location, string Status, string DetectedAt, string Description);
public sealed record WarehouseItemDto(string Id, string IoId, string IoCode, string Bu, string ItemCode, string ItemName, string BatchCode, string Category, int Quantity, string Zone, string Shelf, string Status, string LastMovementAt);
public sealed record ShiftPlanDto(string Id, string ShiftName, string Line, int RequiredWorkers, int AssignedWorkers, double OvertimeHours, string Status);
public sealed record FormRequestDto(string Id, string FormType, string Requester, string Department, string Status, string SubmittedAt, string Summary);
public sealed record NotificationDto(string Id, string Title, string Type, string Severity, string Status, string Time);
public sealed record CameraEventDto(string Id, string CameraCode, string Location, string Type, string Severity, double Confidence, string Time, string? AlertId);
public sealed record CameraDto(string Id, string Name, string Status);
public sealed record UserDto(string Id, string FullName, string Email, string Role, string Department, string Status, string CreatedAt);
public sealed record RoleDto(string Id, string Name);
public sealed record RecommendationDto(string Title, string Detail);
