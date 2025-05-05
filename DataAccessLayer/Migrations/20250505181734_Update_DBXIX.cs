using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Update_DBXIX : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Location_LocationId",
                table: "Schedule");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "Schedule",
                newName: "WarehouseId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedule_LocationId",
                table: "Schedule",
                newName: "IX_Schedule_WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Warehouse_WarehouseId",
                table: "Schedule",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Warehouse_WarehouseId",
                table: "Schedule");

            migrationBuilder.RenameColumn(
                name: "WarehouseId",
                table: "Schedule",
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedule_WarehouseId",
                table: "Schedule",
                newName: "IX_Schedule_LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Location_LocationId",
                table: "Schedule",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id");
        }
    }
}
