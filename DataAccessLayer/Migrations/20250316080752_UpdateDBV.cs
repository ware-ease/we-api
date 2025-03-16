using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDBV : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_Product_ProductId",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "MaxQuantity",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "MinQuantity",
                table: "Inventory");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Inventory",
                newName: "BatchId");

            migrationBuilder.RenameIndex(
                name: "IX_Inventory_ProductId",
                table: "Inventory",
                newName: "IX_Inventory_BatchId");

            migrationBuilder.AddColumn<float>(
                name: "CurrentQuantity",
                table: "Inventory",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "InventoryId",
                table: "Batch",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LocationLog",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NewQuantity = table.Column<float>(type: "real", nullable: false),
                    ChangeInQuantity = table.Column<float>(type: "real", nullable: false),
                    LocationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InventoryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_LocationLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationLog_Inventory_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LocationLog_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationLog_InventoryId",
                table: "LocationLog",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationLog_LocationId",
                table: "LocationLog",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_Batch_BatchId",
                table: "Inventory",
                column: "BatchId",
                principalTable: "Batch",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_Batch_BatchId",
                table: "Inventory");

            migrationBuilder.DropTable(
                name: "LocationLog");

            migrationBuilder.DropColumn(
                name: "CurrentQuantity",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "Batch");

            migrationBuilder.RenameColumn(
                name: "BatchId",
                table: "Inventory",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Inventory_BatchId",
                table: "Inventory",
                newName: "IX_Inventory_ProductId");

            migrationBuilder.AddColumn<int>(
                name: "MaxQuantity",
                table: "Inventory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinQuantity",
                table: "Inventory",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_Product_ProductId",
                table: "Inventory",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id");
        }
    }
}
