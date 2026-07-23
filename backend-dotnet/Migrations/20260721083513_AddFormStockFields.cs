using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartFactory.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFormStockFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "related_item_id",
                table: "form_requests",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "related_quantity",
                table: "form_requests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_form_requests_related_item_id",
                table: "form_requests",
                column: "related_item_id");

            migrationBuilder.AddForeignKey(
                name: "fk_form_requests_warehouse_items_related_item_id",
                table: "form_requests",
                column: "related_item_id",
                principalTable: "warehouse_items",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_form_requests_warehouse_items_related_item_id",
                table: "form_requests");

            migrationBuilder.DropIndex(
                name: "ix_form_requests_related_item_id",
                table: "form_requests");

            migrationBuilder.DropColumn(
                name: "related_item_id",
                table: "form_requests");

            migrationBuilder.DropColumn(
                name: "related_quantity",
                table: "form_requests");
        }
    }
}
