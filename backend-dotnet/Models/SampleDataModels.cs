namespace SmartFactory.Api.Models;

public sealed class SampleData
{
    public List<Role> Roles { get; set; } = [];
    public List<User> Users { get; set; } = [];
    public List<FactoryArea> FactoryAreas { get; set; } = [];
    public List<ProductionLineSource> ProductionLines { get; set; } = [];
    public List<ProductionRecord> ProductionRecords { get; set; } = [];
    public List<Machine> Machines { get; set; } = [];
    public List<MachineIssue> MachineIssues { get; set; } = [];
    public List<WarehouseZone> WarehouseZones { get; set; } = [];
    public List<WarehouseItemSource> WarehouseItems { get; set; } = [];
    public List<GoodsMovement> GoodsMovements { get; set; } = [];
    public List<SafetyAlertSource> SafetyAlerts { get; set; } = [];
    public List<CameraEventSource> CameraEvents { get; set; } = [];
    public List<Employee> Employees { get; set; } = [];
    public List<ShiftPlanSource> ShiftPlans { get; set; } = [];
    public List<FormRequestSource> FormRequests { get; set; } = [];
    public List<NotificationSource> Notifications { get; set; } = [];
    public List<AiRecommendation> AiRecommendations { get; set; } = [];
    public List<Report> Reports { get; set; } = [];
}

public sealed record Role(string Id, string Name, string Description);
public sealed record User(string Id, string FullName, string Email, string RoleId, string Department, string Status, string CreatedAt);
public sealed record FactoryArea(string Id, string Name, string AreaType, string RiskLevel, string Description);
public sealed record ProductionLineSource(string Id, string Name, string AreaId, string Status, int TargetOutput, int ActualOutput, int Efficiency, double DefectRate, int DowntimeMinutes, int AssignedWorkers, string Issue);
public sealed record ProductionRecord(string Id, string LineId, string RecordTime, int TargetOutput, int ActualOutput, int DefectCount, int DowntimeMinutes);
public sealed record Machine(string Id, string MachineCode, string Name, string LineId, string Status, string LastMaintenanceAt);
public sealed record MachineIssue(string Id, string MachineId, string LineId, string Severity, string Status, string Description, string ReportedBy, string ReportedAt);
public sealed record WarehouseZone(string Id, string Name, string ZoneType, int Capacity, int CurrentUsage, string Status);
public sealed record WarehouseItemSource(string Id, string IoId, string IoCode, string Bu, string ItemCode, string ItemName, string BatchCode, string Category, int Quantity, string Unit, string ZoneId, string Shelf, string Status, string LastMovementAt);
public sealed record GoodsMovement(string Id, string ItemId, string? FromZoneId, string ToZoneId, int Quantity, string MovementType, string MovedBy, string MovedAt, string Note);
public sealed record SafetyAlertSource(string Id, string Title, string AlertType, string Severity, string AreaId, string Status, string DetectedAt, string AssignedTo, string Description, string? ActionNote);
public sealed record CameraEventSource(string Id, string CameraCode, string AreaId, string EventType, string Severity, double Confidence, string EventTime, string? RelatedAlertId);
public sealed record Employee(string Id, string EmployeeCode, string FullName, string Department, List<string> SkillTags, string AvailabilityStatus, string? CurrentLineId);
public sealed record ShiftPlanSource(string Id, string ShiftDate, string ShiftName, string LineId, int RequiredWorkers, int AssignedWorkers, double OvertimeHours, string Status);
public sealed record FormRequestSource(string Id, string FormType, string RequesterId, string ApproverId, string Status, string SubmittedAt, string? ApprovedAt, string Summary, string? RejectionReason);
public sealed record NotificationSource(string Id, string Title, string NotificationType, string Severity, string Status, string TargetUserId, string RelatedEntityType, string RelatedEntityId, string CreatedAt);
public sealed record AiRecommendation(string Id, string Module, string Title, string Reason, string ExpectedImpact, string Status, string CreatedAt, string? ReviewedBy);
public sealed record Report(string Id, string ReportType, string Title, string PeriodStart, string PeriodEnd, string Summary, string CreatedBy, string CreatedAt);

public sealed record KpiDto(string Label, string Value, string Detail, string Trend, string Tone);
public sealed record ProductionLineDto(string Id, string Name, string Area, string Status, int TargetOutput, int ActualOutput, int Efficiency, double DefectRate, int DowntimeMinutes, int AssignedWorkers, string Issue);
public sealed record SafetyAlertDto(string Id, string Title, string Type, string Severity, string Location, string Status, string DetectedAt, string Description);
public sealed record WarehouseItemDto(string Id, string IoId, string IoCode, string Bu, string ItemCode, string ItemName, string BatchCode, string Category, int Quantity, string Zone, string Shelf, string Status, string LastMovementAt);
public sealed record ShiftPlanDto(string Id, string ShiftName, string Line, int RequiredWorkers, int AssignedWorkers, double OvertimeHours, string Status);
public sealed record FormRequestDto(string Id, string FormType, string Requester, string Department, string Status, string SubmittedAt, string Summary);
public sealed record NotificationDto(string Id, string Title, string Type, string Severity, string Status, string Time);
public sealed record CameraEventDto(string Id, string CameraCode, string Location, string Type, string Severity, double Confidence, string Time);