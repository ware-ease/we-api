using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDBIX : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryAdjustmentDetail_Inventory_InventoryId",
                table: "InventoryAdjustmentDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationLog_Inventory_InventoryId",
                table: "LocationLog");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationLog_Location_LocationId",
                table: "LocationLog");

            migrationBuilder.DropIndex(
                name: "IX_LocationLog_InventoryId",
                table: "LocationLog");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "LocationLog");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "LocationLog",
                newName: "InventoryLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_LocationLog_LocationId",
                table: "LocationLog",
                newName: "IX_LocationLog_InventoryLocationId");

            migrationBuilder.RenameColumn(
                name: "InventoryId",
                table: "InventoryAdjustmentDetail",
                newName: "LocationLogId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryAdjustmentDetail_InventoryId",
                table: "InventoryAdjustmentDetail",
                newName: "IX_InventoryAdjustmentDetail_LocationLogId");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "GoodRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "GoodNote",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "InventoryLocation",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    InventoryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LocationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryLocation_Inventory_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryLocation_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLocation_InventoryId",
                table: "InventoryLocation",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLocation_LocationId",
                table: "InventoryLocation",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAdjustmentDetail_LocationLog_LocationLogId",
                table: "InventoryAdjustmentDetail",
                column: "LocationLogId",
                principalTable: "LocationLog",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationLog_InventoryLocation_InventoryLocationId",
                table: "LocationLog",
                column: "InventoryLocationId",
                principalTable: "InventoryLocation",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryAdjustmentDetail_LocationLog_LocationLogId",
                table: "InventoryAdjustmentDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationLog_InventoryLocation_InventoryLocationId",
                table: "LocationLog");

            migrationBuilder.DropTable(
                name: "InventoryLocation");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "GoodRequest");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "GoodNote");

            migrationBuilder.RenameColumn(
                name: "InventoryLocationId",
                table: "LocationLog",
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_LocationLog_InventoryLocationId",
                table: "LocationLog",
                newName: "IX_LocationLog_LocationId");

            migrationBuilder.RenameColumn(
                name: "LocationLogId",
                table: "InventoryAdjustmentDetail",
                newName: "InventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryAdjustmentDetail_LocationLogId",
                table: "InventoryAdjustmentDetail",
                newName: "IX_InventoryAdjustmentDetail_InventoryId");

            migrationBuilder.AddColumn<string>(
                name: "InventoryId",
                table: "LocationLog",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_LocationLog_InventoryId",
                table: "LocationLog",
                column: "InventoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAdjustmentDetail_Inventory_InventoryId",
                table: "InventoryAdjustmentDetail",
                column: "InventoryId",
                principalTable: "Inventory",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationLog_Inventory_InventoryId",
                table: "LocationLog",
                column: "InventoryId",
                principalTable: "Inventory",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationLog_Location_LocationId",
                table: "LocationLog",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id");
        }
    }
}
