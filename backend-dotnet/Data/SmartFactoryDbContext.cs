using Microsoft.EntityFrameworkCore;
using SmartFactory.Api.Models.Database;

namespace SmartFactory.Api.Data;

/// <summary>
/// Code-first EF Core model for the SmartFactory SQL Server database. The 31
/// existing POCO entities are the single source of truth; snake_case table/column
/// names (matching the original schema.sql and the design docs) are applied by
/// UseSnakeCaseNamingConvention() where this context is registered.
/// </summary>
public sealed class SmartFactoryDbContext(DbContextOptions<SmartFactoryDbContext> options) : DbContext(options)
{
    // Identity & access
    public DbSet<RoleEntity> Roles => Set<RoleEntity>();
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<PermissionEntity> Permissions => Set<PermissionEntity>();
    public DbSet<RolePermissionEntity> RolePermissions => Set<RolePermissionEntity>();

    // Factory & production
    public DbSet<FactoryAreaEntity> FactoryAreas => Set<FactoryAreaEntity>();
    public DbSet<ProductionLineEntity> ProductionLines => Set<ProductionLineEntity>();
    public DbSet<ProductionRecordEntity> ProductionRecords => Set<ProductionRecordEntity>();
    public DbSet<MachineEntity> Machines => Set<MachineEntity>();
    public DbSet<MachineIssueEntity> MachineIssues => Set<MachineIssueEntity>();

    // Warehouse
    public DbSet<WarehouseZoneEntity> WarehouseZones => Set<WarehouseZoneEntity>();
    public DbSet<BusinessUnitEntity> BusinessUnits => Set<BusinessUnitEntity>();
    public DbSet<IoMasterEntity> IoMasters => Set<IoMasterEntity>();
    public DbSet<WarehouseItemEntity> WarehouseItems => Set<WarehouseItemEntity>();
    public DbSet<WarehouseItemIoLinkEntity> WarehouseItemIoLinks => Set<WarehouseItemIoLinkEntity>();
    public DbSet<GoodsMovementEntity> GoodsMovements => Set<GoodsMovementEntity>();

    // Safety & camera
    public DbSet<SafetyAlertEntity> SafetyAlerts => Set<SafetyAlertEntity>();
    public DbSet<CameraEventEntity> CameraEvents => Set<CameraEventEntity>();
    public DbSet<SafetyAlertCameraEventEntity> SafetyAlertCameraEvents => Set<SafetyAlertCameraEventEntity>();

    // Workforce
    public DbSet<EmployeeEntity> Employees => Set<EmployeeEntity>();
    public DbSet<SkillEntity> Skills => Set<SkillEntity>();
    public DbSet<EmployeeSkillEntity> EmployeeSkills => Set<EmployeeSkillEntity>();
    public DbSet<ShiftPlanEntity> ShiftPlans => Set<ShiftPlanEntity>();
    public DbSet<ShiftPlanAssignmentEntity> ShiftPlanAssignments => Set<ShiftPlanAssignmentEntity>();

    // Workflow
    public DbSet<FormRequestEntity> FormRequests => Set<FormRequestEntity>();
    public DbSet<FormApprovalStepEntity> FormApprovalSteps => Set<FormApprovalStepEntity>();
    public DbSet<NotificationEntity> Notifications => Set<NotificationEntity>();
    public DbSet<NotificationLinkEntity> NotificationLinks => Set<NotificationLinkEntity>();

    // AI & reporting
    public DbSet<AiRecommendationEntity> AiRecommendations => Set<AiRecommendationEntity>();
    public DbSet<AiRecommendationLinkEntity> AiRecommendationLinks => Set<AiRecommendationLinkEntity>();
    public DbSet<ReportEntity> Reports => Set<ReportEntity>();
    public DbSet<ReportSourceLinkEntity> ReportSourceLinks => Set<ReportSourceLinkEntity>();

    // Configuration & master data
    public DbSet<CameraEntity> Cameras => Set<CameraEntity>();
    public DbSet<AppSettingEntity> AppSettings => Set<AppSettingEntity>();

    // Bound every string column so PKs and unique columns can be indexed by SQL Server
    // (nvarchar(max) cannot be a key). Free-text columns are widened individually below.
    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<string>().HaveMaxLength(256);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- Composite keys (link tables) ---
        modelBuilder.Entity<RolePermissionEntity>().HasKey(x => new { x.RoleId, x.PermissionId });
        modelBuilder.Entity<WarehouseItemIoLinkEntity>().HasKey(x => new { x.ItemId, x.IoMasterId });
        modelBuilder.Entity<SafetyAlertCameraEventEntity>().HasKey(x => new { x.AlertId, x.CameraEventId });
        modelBuilder.Entity<EmployeeSkillEntity>().HasKey(x => new { x.EmployeeId, x.SkillId });
        modelBuilder.Entity<ShiftPlanAssignmentEntity>().HasKey(x => new { x.ShiftId, x.EmployeeId });
        modelBuilder.Entity<NotificationLinkEntity>().HasKey(x => new { x.NotificationId, x.EntityType, x.EntityId });
        modelBuilder.Entity<AiRecommendationLinkEntity>().HasKey(x => new { x.RecommendationId, x.EntityType, x.EntityId });
        modelBuilder.Entity<ReportSourceLinkEntity>().HasKey(x => new { x.ReportId, x.SourceType, x.SourceId });

        // Natural-key tables for configuration & master data.
        modelBuilder.Entity<CameraEntity>().HasKey(x => x.CameraCode);
        modelBuilder.Entity<AppSettingEntity>().HasKey(x => x.SettingKey);

        // --- Unique indexes (UNIQUE columns in the original schema) ---
        modelBuilder.Entity<RoleEntity>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<UserEntity>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<PermissionEntity>().HasIndex(x => x.PermissionCode).IsUnique();
        modelBuilder.Entity<MachineEntity>().HasIndex(x => x.MachineCode).IsUnique();
        modelBuilder.Entity<BusinessUnitEntity>().HasIndex(x => x.BuCode).IsUnique();
        modelBuilder.Entity<IoMasterEntity>().HasIndex(x => x.IoId).IsUnique();
        modelBuilder.Entity<IoMasterEntity>().HasIndex(x => x.IoCode).IsUnique();
        modelBuilder.Entity<WarehouseItemEntity>().HasIndex(x => x.ItemCode).IsUnique();
        modelBuilder.Entity<EmployeeEntity>().HasIndex(x => x.EmployeeCode).IsUnique();
        modelBuilder.Entity<SkillEntity>().HasIndex(x => x.SkillCode).IsUnique();

        // --- Free-text columns widened to nvarchar(max) ---
        modelBuilder.Entity<RoleEntity>().Property(x => x.Description).HasColumnType("nvarchar(max)");
        modelBuilder.Entity<PermissionEntity>().Property(x => x.Description).HasColumnType("nvarchar(max)");
        modelBuilder.Entity<FactoryAreaEntity>().Property(x => x.Description).HasColumnType("nvarchar(max)");
        modelBuilder.Entity<ProductionLineEntity>().Property(x => x.Issue).HasColumnType("nvarchar(max)");
        modelBuilder.Entity<MachineIssueEntity>().Property(x => x.Description).HasColumnType("nvarchar(max)");
        modelBuilder.Entity<SafetyAlertEntity>().Property(x => x.Description).HasColumnType("nvarchar(max)");
        modelBuilder.Entity<SafetyAlertEntity>().Property(x => x.ActionNote).HasColumnType("nvarchar(max)");
        modelBuilder.Entity<GoodsMovementEntity>().Property(x => x.Note).HasColumnType("nvarchar(max)");
        modelBuilder.Entity<EmployeeEntity>().Property(x => x.SkillTags).HasColumnType("nvarchar(max)");
        modelBuilder.Entity<FormRequestEntity>().Property(x => x.Summary).HasColumnType("nvarchar(max)");
        modelBuilder.Entity<FormRequestEntity>().Property(x => x.RejectionReason).HasColumnType("nvarchar(max)");
        modelBuilder.Entity<FormApprovalStepEntity>().Property(x => x.Note).HasColumnType("nvarchar(max)");
        modelBuilder.Entity<AiRecommendationEntity>().Property(x => x.Reason).HasColumnType("nvarchar(max)");
        modelBuilder.Entity<AiRecommendationEntity>().Property(x => x.ExpectedImpact).HasColumnType("nvarchar(max)");
        modelBuilder.Entity<ReportEntity>().Property(x => x.Summary).HasColumnType("nvarchar(max)");

        // --- Foreign keys (no navigation properties; Restrict matches the original no-cascade schema) ---
        modelBuilder.Entity<UserEntity>().HasOne<RoleEntity>().WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RolePermissionEntity>().HasOne<RoleEntity>().WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<RolePermissionEntity>().HasOne<PermissionEntity>().WithMany().HasForeignKey(x => x.PermissionId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProductionLineEntity>().HasOne<FactoryAreaEntity>().WithMany().HasForeignKey(x => x.AreaId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ProductionRecordEntity>().HasOne<ProductionLineEntity>().WithMany().HasForeignKey(x => x.LineId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<MachineEntity>().HasOne<ProductionLineEntity>().WithMany().HasForeignKey(x => x.LineId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MachineIssueEntity>().HasOne<MachineEntity>().WithMany().HasForeignKey(x => x.MachineId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<MachineIssueEntity>().HasOne<ProductionLineEntity>().WithMany().HasForeignKey(x => x.LineId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<MachineIssueEntity>().HasOne<UserEntity>().WithMany().HasForeignKey(x => x.ReportedBy).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<IoMasterEntity>().HasOne<BusinessUnitEntity>().WithMany().HasForeignKey(x => x.BuId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<WarehouseItemEntity>().HasOne<WarehouseZoneEntity>().WithMany().HasForeignKey(x => x.ZoneId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WarehouseItemIoLinkEntity>().HasOne<WarehouseItemEntity>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<WarehouseItemIoLinkEntity>().HasOne<IoMasterEntity>().WithMany().HasForeignKey(x => x.IoMasterId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GoodsMovementEntity>().HasOne<WarehouseItemEntity>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<GoodsMovementEntity>().HasOne<WarehouseZoneEntity>().WithMany().HasForeignKey(x => x.FromZoneId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<GoodsMovementEntity>().HasOne<WarehouseZoneEntity>().WithMany().HasForeignKey(x => x.ToZoneId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<GoodsMovementEntity>().HasOne<UserEntity>().WithMany().HasForeignKey(x => x.MovedBy).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SafetyAlertEntity>().HasOne<FactoryAreaEntity>().WithMany().HasForeignKey(x => x.AreaId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<SafetyAlertEntity>().HasOne<UserEntity>().WithMany().HasForeignKey(x => x.AssignedTo).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CameraEventEntity>().HasOne<FactoryAreaEntity>().WithMany().HasForeignKey(x => x.AreaId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<CameraEventEntity>().HasOne<SafetyAlertEntity>().WithMany().HasForeignKey(x => x.RelatedAlertId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SafetyAlertCameraEventEntity>().HasOne<SafetyAlertEntity>().WithMany().HasForeignKey(x => x.AlertId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<SafetyAlertCameraEventEntity>().HasOne<CameraEventEntity>().WithMany().HasForeignKey(x => x.CameraEventId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeEntity>().HasOne<ProductionLineEntity>().WithMany().HasForeignKey(x => x.CurrentLineId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeSkillEntity>().HasOne<EmployeeEntity>().WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeSkillEntity>().HasOne<SkillEntity>().WithMany().HasForeignKey(x => x.SkillId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ShiftPlanEntity>().HasOne<ProductionLineEntity>().WithMany().HasForeignKey(x => x.LineId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ShiftPlanAssignmentEntity>().HasOne<ShiftPlanEntity>().WithMany().HasForeignKey(x => x.ShiftId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ShiftPlanAssignmentEntity>().HasOne<EmployeeEntity>().WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FormRequestEntity>().HasOne<UserEntity>().WithMany().HasForeignKey(x => x.RequesterId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FormRequestEntity>().HasOne<UserEntity>().WithMany().HasForeignKey(x => x.ApproverId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FormRequestEntity>().HasOne<WarehouseItemEntity>().WithMany().HasForeignKey(x => x.RelatedItemId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FormApprovalStepEntity>().HasOne<FormRequestEntity>().WithMany().HasForeignKey(x => x.FormId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FormApprovalStepEntity>().HasOne<UserEntity>().WithMany().HasForeignKey(x => x.ApproverId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NotificationEntity>().HasOne<UserEntity>().WithMany().HasForeignKey(x => x.TargetUserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<NotificationLinkEntity>().HasOne<NotificationEntity>().WithMany().HasForeignKey(x => x.NotificationId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AiRecommendationEntity>().HasOne<UserEntity>().WithMany().HasForeignKey(x => x.ReviewedBy).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AiRecommendationLinkEntity>().HasOne<AiRecommendationEntity>().WithMany().HasForeignKey(x => x.RecommendationId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ReportEntity>().HasOne<UserEntity>().WithMany().HasForeignKey(x => x.CreatedBy).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ReportSourceLinkEntity>().HasOne<ReportEntity>().WithMany().HasForeignKey(x => x.ReportId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CameraEntity>().HasOne<FactoryAreaEntity>().WithMany().HasForeignKey(x => x.AreaId).OnDelete(DeleteBehavior.Restrict);
    }
}
