using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupAction_Permission_PermissionId",
                table: "GroupAction");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCheck_StockCard_StockCardId",
                table: "InventoryCheck");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCheckDetail_StockCard_StockCardId",
                table: "InventoryCheckDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptDetail_Brand_BrandId",
                table: "ReceiptDetail");

            migrationBuilder.DropTable(
                name: "StockCardDetail");

            migrationBuilder.DropTable(
                name: "StockCard");

            migrationBuilder.DropIndex(
                name: "IX_InventoryCheckDetail_StockCardId",
                table: "InventoryCheckDetail");

            migrationBuilder.DropIndex(
                name: "IX_GroupAction_PermissionId",
                table: "GroupAction");

            migrationBuilder.DropColumn(
                name: "StockCardId",
                table: "InventoryCheckDetail");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "PermissionId",
                table: "GroupAction");

            migrationBuilder.RenameColumn(
                name: "BrandId",
                table: "ReceiptDetail",
                newName: "BatchId");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptDetail_BrandId",
                table: "ReceiptDetail",
                newName: "IX_ReceiptDetail_BatchId");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Permission",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CellBatchId",
                table: "InventoryCheckDetail",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CellBatch",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CellId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_CellBatch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CellBatch_Cell_CellId",
                        column: x => x.CellId,
                        principalTable: "Cell",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockBook",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WarehouseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_StockBook", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockBook_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockBook_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InOutDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Stock = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CellBatchId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BatchId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_InOutDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InOutDetail_Batch_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batch",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InOutDetail_CellBatch_CellBatchId",
                        column: x => x.CellBatchId,
                        principalTable: "CellBatch",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCheckDetail_CellBatchId",
                table: "InventoryCheckDetail",
                column: "CellBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_CellBatch_CellId",
                table: "CellBatch",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "IX_InOutDetail_BatchId",
                table: "InOutDetail",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_InOutDetail_CellBatchId",
                table: "InOutDetail",
                column: "CellBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_StockBook_ProductId",
                table: "StockBook",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockBook_WarehouseId",
                table: "StockBook",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryCheck_CellBatch_StockCardId",
                table: "InventoryCheck",
                column: "StockCardId",
                principalTable: "CellBatch",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryCheckDetail_CellBatch_CellBatchId",
                table: "InventoryCheckDetail",
                column: "CellBatchId",
                principalTable: "CellBatch",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptDetail_Batch_BatchId",
                table: "ReceiptDetail",
                column: "BatchId",
                principalTable: "Batch",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCheck_CellBatch_StockCardId",
                table: "InventoryCheck");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCheckDetail_CellBatch_CellBatchId",
                table: "InventoryCheckDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptDetail_Batch_BatchId",
                table: "ReceiptDetail");

            migrationBuilder.DropTable(
                name: "InOutDetail");

            migrationBuilder.DropTable(
                name: "StockBook");

            migrationBuilder.DropTable(
                name: "CellBatch");

            migrationBuilder.DropIndex(
                name: "IX_InventoryCheckDetail_CellBatchId",
                table: "InventoryCheckDetail");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "CellBatchId",
                table: "InventoryCheckDetail");

            migrationBuilder.RenameColumn(
                name: "BatchId",
                table: "ReceiptDetail",
                newName: "BrandId");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptDetail_BatchId",
                table: "ReceiptDetail",
                newName: "IX_ReceiptDetail_BrandId");

            migrationBuilder.AddColumn<string>(
                name: "StockCardId",
                table: "InventoryCheckDetail",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Inventory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PermissionId",
                table: "GroupAction",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StockCard",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CellId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockCard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockCard_Cell_CellId",
                        column: x => x.CellId,
                        principalTable: "Cell",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockCardDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BatchId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StockCardId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Stock = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockCardDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockCardDetail_Batch_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batch",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockCardDetail_StockCard_StockCardId",
                        column: x => x.StockCardId,
                        principalTable: "StockCard",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCheckDetail_StockCardId",
                table: "InventoryCheckDetail",
                column: "StockCardId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAction_PermissionId",
                table: "GroupAction",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_StockCard_CellId",
                table: "StockCard",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "IX_StockCardDetail_BatchId",
                table: "StockCardDetail",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_StockCardDetail_StockCardId",
                table: "StockCardDetail",
                column: "StockCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupAction_Permission_PermissionId",
                table: "GroupAction",
                column: "PermissionId",
                principalTable: "Permission",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryCheck_StockCard_StockCardId",
                table: "InventoryCheck",
                column: "StockCardId",
                principalTable: "StockCard",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryCheckDetail_StockCard_StockCardId",
                table: "InventoryCheckDetail",
                column: "StockCardId",
                principalTable: "StockCard",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptDetail_Brand_BrandId",
                table: "ReceiptDetail",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "Id");
        }
    }
}
