using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartFactory.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCamerasAndAppSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "app_settings",
                columns: table => new
                {
                    setting_key = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    setting_value = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    value_type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    category = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    updated_at = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_settings", x => x.setting_key);
                });

            migrationBuilder.CreateTable(
                name: "cameras",
                columns: table => new
                {
                    camera_code = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    area_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    status = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cameras", x => x.camera_code);
                    table.ForeignKey(
                        name: "fk_cameras_factory_areas_area_id",
                        column: x => x.area_id,
                        principalTable: "factory_areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cameras_area_id",
                table: "cameras",
                column: "area_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "app_settings");

            migrationBuilder.DropTable(
                name: "cameras");
        }
    }
}
