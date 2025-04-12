using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDBXVII : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryAdjustmentDetail_LocationLog_LocationLogId",
                table: "InventoryAdjustmentDetail");

            migrationBuilder.DropColumn(
                name: "ExpDate",
                table: "Batch");

            migrationBuilder.RenameColumn(
                name: "LocationLogId",
                table: "InventoryAdjustmentDetail",
                newName: "InventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryAdjustmentDetail_LocationLogId",
                table: "InventoryAdjustmentDetail",
                newName: "IX_InventoryAdjustmentDetail_InventoryId");

            migrationBuilder.RenameColumn(
                name: "MfgDate",
                table: "Batch",
                newName: "InboundDate");

            migrationBuilder.AddColumn<int>(
                name: "QuantityType",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAdjustmentDetail_Inventory_InventoryId",
                table: "InventoryAdjustmentDetail",
                column: "InventoryId",
                principalTable: "Inventory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryAdjustmentDetail_Inventory_InventoryId",
                table: "InventoryAdjustmentDetail");

            migrationBuilder.DropColumn(
                name: "QuantityType",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "InventoryId",
                table: "InventoryAdjustmentDetail",
                newName: "LocationLogId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryAdjustmentDetail_InventoryId",
                table: "InventoryAdjustmentDetail",
                newName: "IX_InventoryAdjustmentDetail_LocationLogId");

            migrationBuilder.RenameColumn(
                name: "InboundDate",
                table: "Batch",
                newName: "MfgDate");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExpDate",
                table: "Batch",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAdjustmentDetail_LocationLog_LocationLogId",
                table: "InventoryAdjustmentDetail",
                column: "LocationLogId",
                principalTable: "LocationLog",
                principalColumn: "Id");
        }
    }
}
