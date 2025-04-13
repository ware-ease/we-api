using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class InventoryCount_Update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryCount_ScheduleId",
                table: "InventoryCount");

            migrationBuilder.AlterColumn<string>(
                name: "ScheduleId",
                table: "InventoryCount",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCount_ScheduleId",
                table: "InventoryCount",
                column: "ScheduleId",
                unique: true,
                filter: "[ScheduleId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryCount_ScheduleId",
                table: "InventoryCount");

            migrationBuilder.AlterColumn<string>(
                name: "ScheduleId",
                table: "InventoryCount",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCount_ScheduleId",
                table: "InventoryCount",
                column: "ScheduleId",
                unique: true);
        }
    }
}
