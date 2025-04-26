using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Update_DBXVIII : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCount_Schedule_ScheduleId",
                table: "InventoryCount");

            migrationBuilder.DropIndex(
                name: "IX_InventoryCount_ScheduleId",
                table: "InventoryCount");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "InventoryCount");

            migrationBuilder.AddColumn<string>(
                name: "InventoryCountId",
                table: "Schedule",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "InventoryCountDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_InventoryCountId",
                table: "Schedule",
                column: "InventoryCountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_InventoryCount_InventoryCountId",
                table: "Schedule",
                column: "InventoryCountId",
                principalTable: "InventoryCount",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_InventoryCount_InventoryCountId",
                table: "Schedule");

            migrationBuilder.DropIndex(
                name: "IX_Schedule_InventoryCountId",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "InventoryCountId",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "InventoryCountDetail");

            migrationBuilder.AddColumn<string>(
                name: "ScheduleId",
                table: "InventoryCount",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCount_ScheduleId",
                table: "InventoryCount",
                column: "ScheduleId",
                unique: true,
                filter: "[ScheduleId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryCount_Schedule_ScheduleId",
                table: "InventoryCount",
                column: "ScheduleId",
                principalTable: "Schedule",
                principalColumn: "Id");
        }
    }
}
