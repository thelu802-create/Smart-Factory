using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartFactory.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "business_units",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    bu_code = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    bu_name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    status = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_business_units", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "factory_areas",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    area_type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    risk_level = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_factory_areas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    permission_code = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    module = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    action = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "skills",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    skill_code = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    skill_name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    skill_group = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_skills", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "warehouse_zones",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    zone_type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    capacity = table.Column<int>(type: "int", nullable: false),
                    current_usage = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_warehouse_zones", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "io_masters",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    io_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    io_code = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    bu_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    status = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_io_masters", x => x.id);
                    table.ForeignKey(
                        name: "fk_io_masters_business_units_bu_id",
                        column: x => x.bu_id,
                        principalTable: "business_units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "production_lines",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    area_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    status = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    target_output = table.Column<int>(type: "int", nullable: false),
                    actual_output = table.Column<int>(type: "int", nullable: false),
                    efficiency = table.Column<int>(type: "int", nullable: false),
                    defect_rate = table.Column<double>(type: "float", nullable: false),
                    downtime_minutes = table.Column<int>(type: "int", nullable: false),
                    assigned_workers = table.Column<int>(type: "int", nullable: false),
                    issue = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_production_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_production_lines_factory_areas_area_id",
                        column: x => x.area_id,
                        principalTable: "factory_areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    role_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    permission_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    granted_at = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_permissions", x => new { x.role_id, x.permission_id });
                    table.ForeignKey(
                        name: "fk_role_permissions_permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_role_permissions_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    role_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    department = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    status = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    created_at = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "warehouse_items",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    io_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    io_code = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    bu = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    item_code = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    item_name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    batch_code = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    category = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unit = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    zone_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    shelf = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    status = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    last_movement_at = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_warehouse_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_warehouse_items_warehouse_zones_zone_id",
                        column: x => x.zone_id,
                        principalTable: "warehouse_zones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    employee_code = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    department = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    skill_tags = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false),
                    availability_status = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    current_line_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employees", x => x.id);
                    table.ForeignKey(
                        name: "fk_employees_production_lines_current_line_id",
                        column: x => x.current_line_id,
                        principalTable: "production_lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "machines",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    machine_code = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    line_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    status = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    last_maintenance_at = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_machines", x => x.id);
                    table.ForeignKey(
                        name: "fk_machines_production_lines_line_id",
                        column: x => x.line_id,
                        principalTable: "production_lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "production_records",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    line_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    record_time = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    target_output = table.Column<int>(type: "int", nullable: false),
                    actual_output = table.Column<int>(type: "int", nullable: false),
                    defect_count = table.Column<int>(type: "int", nullable: false),
                    downtime_minutes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_production_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_production_records_production_lines_line_id",
                        column: x => x.line_id,
                        principalTable: "production_lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "shift_plans",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    shift_date = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    shift_name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    line_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    required_workers = table.Column<int>(type: "int", nullable: false),
                    assigned_workers = table.Column<int>(type: "int", nullable: false),
                    overtime_hours = table.Column<double>(type: "float", nullable: false),
                    status = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shift_plans", x => x.id);
                    table.ForeignKey(
                        name: "fk_shift_plans_production_lines_line_id",
                        column: x => x.line_id,
                        principalTable: "production_lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ai_recommendations",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    module = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    reason = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false),
                    expected_impact = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false),
                    status = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    created_at = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    reviewed_by = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ai_recommendations", x => x.id);
                    table.ForeignKey(
                        name: "fk_ai_recommendations_users_reviewed_by",
                        column: x => x.reviewed_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "form_requests",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    form_type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    requester_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    approver_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    status = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    submitted_at = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    approved_at = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    summary = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false),
                    rejection_reason = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_form_requests", x => x.id);
                    table.ForeignKey(
                        name: "fk_form_requests_users_approver_id",
                        column: x => x.approver_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_form_requests_users_requester_id",
                        column: x => x.requester_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    notification_type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    severity = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    status = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    target_user_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    related_entity_type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    related_entity_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    created_at = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notifications", x => x.id);
                    table.ForeignKey(
                        name: "fk_notifications_users_target_user_id",
                        column: x => x.target_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reports",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    report_type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    period_start = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    period_end = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    summary = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    created_at = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reports", x => x.id);
                    table.ForeignKey(
                        name: "fk_reports_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "safety_alerts",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    alert_type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    severity = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    area_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    status = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    detected_at = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    assigned_to = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false),
                    action_note = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_safety_alerts", x => x.id);
                    table.ForeignKey(
                        name: "fk_safety_alerts_factory_areas_area_id",
                        column: x => x.area_id,
                        principalTable: "factory_areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_safety_alerts_users_assigned_to",
                        column: x => x.assigned_to,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "goods_movements",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    item_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    from_zone_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    to_zone_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    movement_type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    moved_by = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    moved_at = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    note = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_goods_movements", x => x.id);
                    table.ForeignKey(
                        name: "fk_goods_movements_users_moved_by",
                        column: x => x.moved_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_goods_movements_warehouse_items_item_id",
                        column: x => x.item_id,
                        principalTable: "warehouse_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_goods_movements_warehouse_zones_from_zone_id",
                        column: x => x.from_zone_id,
                        principalTable: "warehouse_zones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_goods_movements_warehouse_zones_to_zone_id",
                        column: x => x.to_zone_id,
                        principalTable: "warehouse_zones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "warehouse_item_io_links",
                columns: table => new
                {
                    item_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    io_master_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    link_type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    effective_from = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    effective_to = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_warehouse_item_io_links", x => new { x.item_id, x.io_master_id });
                    table.ForeignKey(
                        name: "fk_warehouse_item_io_links_io_masters_io_master_id",
                        column: x => x.io_master_id,
                        principalTable: "io_masters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_warehouse_item_io_links_warehouse_items_item_id",
                        column: x => x.item_id,
                        principalTable: "warehouse_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "employee_skills",
                columns: table => new
                {
                    employee_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    skill_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    proficiency_level = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    certified_at = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    expires_at = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee_skills", x => new { x.employee_id, x.skill_id });
                    table.ForeignKey(
                        name: "fk_employee_skills_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_employee_skills_skills_skill_id",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "machine_issues",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    machine_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    line_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    severity = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    status = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: false),
                    reported_by = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    reported_at = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_machine_issues", x => x.id);
                    table.ForeignKey(
                        name: "fk_machine_issues_machines_machine_id",
                        column: x => x.machine_id,
                        principalTable: "machines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_machine_issues_production_lines_line_id",
                        column: x => x.line_id,
                        principalTable: "production_lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_machine_issues_users_reported_by",
                        column: x => x.reported_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "shift_plan_assignments",
                columns: table => new
                {
                    shift_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    employee_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    assignment_role = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    assignment_status = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    assigned_at = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shift_plan_assignments", x => new { x.shift_id, x.employee_id });
                    table.ForeignKey(
                        name: "fk_shift_plan_assignments_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_shift_plan_assignments_shift_plans_shift_id",
                        column: x => x.shift_id,
                        principalTable: "shift_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ai_recommendation_links",
                columns: table => new
                {
                    recommendation_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    entity_type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    entity_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    link_reason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ai_recommendation_links", x => new { x.recommendation_id, x.entity_type, x.entity_id });
                    table.ForeignKey(
                        name: "fk_ai_recommendation_links_ai_recommendations_recommendation_id",
                        column: x => x.recommendation_id,
                        principalTable: "ai_recommendations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "form_approval_steps",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    form_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    step_order = table.Column<int>(type: "int", nullable: false),
                    approver_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    status = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    action_at = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    note = table.Column<string>(type: "nvarchar(max)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_form_approval_steps", x => x.id);
                    table.ForeignKey(
                        name: "fk_form_approval_steps_form_requests_form_id",
                        column: x => x.form_id,
                        principalTable: "form_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_form_approval_steps_users_approver_id",
                        column: x => x.approver_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notification_links",
                columns: table => new
                {
                    notification_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    entity_type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    entity_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    link_role = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_links", x => new { x.notification_id, x.entity_type, x.entity_id });
                    table.ForeignKey(
                        name: "fk_notification_links_notifications_notification_id",
                        column: x => x.notification_id,
                        principalTable: "notifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "report_source_links",
                columns: table => new
                {
                    report_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    source_type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    source_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    included_at = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_report_source_links", x => new { x.report_id, x.source_type, x.source_id });
                    table.ForeignKey(
                        name: "fk_report_source_links_reports_report_id",
                        column: x => x.report_id,
                        principalTable: "reports",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "camera_events",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    camera_code = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    area_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    event_type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    severity = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    confidence = table.Column<double>(type: "float", nullable: false),
                    event_time = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    related_alert_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_camera_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_camera_events_factory_areas_area_id",
                        column: x => x.area_id,
                        principalTable: "factory_areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_camera_events_safety_alerts_related_alert_id",
                        column: x => x.related_alert_id,
                        principalTable: "safety_alerts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "safety_alert_camera_events",
                columns: table => new
                {
                    alert_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    camera_event_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    relation_type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    linked_at = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_safety_alert_camera_events", x => new { x.alert_id, x.camera_event_id });
                    table.ForeignKey(
                        name: "fk_safety_alert_camera_events_camera_events_camera_event_id",
                        column: x => x.camera_event_id,
                        principalTable: "camera_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_safety_alert_camera_events_safety_alerts_alert_id",
                        column: x => x.alert_id,
                        principalTable: "safety_alerts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ai_recommendations_reviewed_by",
                table: "ai_recommendations",
                column: "reviewed_by");

            migrationBuilder.CreateIndex(
                name: "ix_business_units_bu_code",
                table: "business_units",
                column: "bu_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_camera_events_area_id",
                table: "camera_events",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "ix_camera_events_related_alert_id",
                table: "camera_events",
                column: "related_alert_id");

            migrationBuilder.CreateIndex(
                name: "ix_employee_skills_skill_id",
                table: "employee_skills",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "ix_employees_current_line_id",
                table: "employees",
                column: "current_line_id");

            migrationBuilder.CreateIndex(
                name: "ix_employees_employee_code",
                table: "employees",
                column: "employee_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_form_approval_steps_approver_id",
                table: "form_approval_steps",
                column: "approver_id");

            migrationBuilder.CreateIndex(
                name: "ix_form_approval_steps_form_id",
                table: "form_approval_steps",
                column: "form_id");

            migrationBuilder.CreateIndex(
                name: "ix_form_requests_approver_id",
                table: "form_requests",
                column: "approver_id");

            migrationBuilder.CreateIndex(
                name: "ix_form_requests_requester_id",
                table: "form_requests",
                column: "requester_id");

            migrationBuilder.CreateIndex(
                name: "ix_goods_movements_from_zone_id",
                table: "goods_movements",
                column: "from_zone_id");

            migrationBuilder.CreateIndex(
                name: "ix_goods_movements_item_id",
                table: "goods_movements",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "ix_goods_movements_moved_by",
                table: "goods_movements",
                column: "moved_by");

            migrationBuilder.CreateIndex(
                name: "ix_goods_movements_to_zone_id",
                table: "goods_movements",
                column: "to_zone_id");

            migrationBuilder.CreateIndex(
                name: "ix_io_masters_bu_id",
                table: "io_masters",
                column: "bu_id");

            migrationBuilder.CreateIndex(
                name: "ix_io_masters_io_code",
                table: "io_masters",
                column: "io_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_io_masters_io_id",
                table: "io_masters",
                column: "io_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_machine_issues_line_id",
                table: "machine_issues",
                column: "line_id");

            migrationBuilder.CreateIndex(
                name: "ix_machine_issues_machine_id",
                table: "machine_issues",
                column: "machine_id");

            migrationBuilder.CreateIndex(
                name: "ix_machine_issues_reported_by",
                table: "machine_issues",
                column: "reported_by");

            migrationBuilder.CreateIndex(
                name: "ix_machines_line_id",
                table: "machines",
                column: "line_id");

            migrationBuilder.CreateIndex(
                name: "ix_machines_machine_code",
                table: "machines",
                column: "machine_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_notifications_target_user_id",
                table: "notifications",
                column: "target_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_permissions_permission_code",
                table: "permissions",
                column: "permission_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_production_lines_area_id",
                table: "production_lines",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "ix_production_records_line_id",
                table: "production_records",
                column: "line_id");

            migrationBuilder.CreateIndex(
                name: "ix_reports_created_by",
                table: "reports",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_permission_id",
                table: "role_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "ix_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_safety_alert_camera_events_camera_event_id",
                table: "safety_alert_camera_events",
                column: "camera_event_id");

            migrationBuilder.CreateIndex(
                name: "ix_safety_alerts_area_id",
                table: "safety_alerts",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "ix_safety_alerts_assigned_to",
                table: "safety_alerts",
                column: "assigned_to");

            migrationBuilder.CreateIndex(
                name: "ix_shift_plan_assignments_employee_id",
                table: "shift_plan_assignments",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_shift_plans_line_id",
                table: "shift_plans",
                column: "line_id");

            migrationBuilder.CreateIndex(
                name: "ix_skills_skill_code",
                table: "skills",
                column: "skill_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_role_id",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_warehouse_item_io_links_io_master_id",
                table: "warehouse_item_io_links",
                column: "io_master_id");

            migrationBuilder.CreateIndex(
                name: "ix_warehouse_items_item_code",
                table: "warehouse_items",
                column: "item_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_warehouse_items_zone_id",
                table: "warehouse_items",
                column: "zone_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ai_recommendation_links");

            migrationBuilder.DropTable(
                name: "employee_skills");

            migrationBuilder.DropTable(
                name: "form_approval_steps");

            migrationBuilder.DropTable(
                name: "goods_movements");

            migrationBuilder.DropTable(
                name: "machine_issues");

            migrationBuilder.DropTable(
                name: "notification_links");

            migrationBuilder.DropTable(
                name: "production_records");

            migrationBuilder.DropTable(
                name: "report_source_links");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "safety_alert_camera_events");

            migrationBuilder.DropTable(
                name: "shift_plan_assignments");

            migrationBuilder.DropTable(
                name: "warehouse_item_io_links");

            migrationBuilder.DropTable(
                name: "ai_recommendations");

            migrationBuilder.DropTable(
                name: "skills");

            migrationBuilder.DropTable(
                name: "form_requests");

            migrationBuilder.DropTable(
                name: "machines");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "reports");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "camera_events");

            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.DropTable(
                name: "shift_plans");

            migrationBuilder.DropTable(
                name: "io_masters");

            migrationBuilder.DropTable(
                name: "warehouse_items");

            migrationBuilder.DropTable(
                name: "safety_alerts");

            migrationBuilder.DropTable(
                name: "production_lines");

            migrationBuilder.DropTable(
                name: "business_units");

            migrationBuilder.DropTable(
                name: "warehouse_zones");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "factory_areas");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
