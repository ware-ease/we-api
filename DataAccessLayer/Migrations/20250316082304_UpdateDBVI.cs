using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDBVI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ErrorTicket_InventoryCheck_InventoryCheckId",
                table: "ErrorTicket");

            migrationBuilder.DropTable(
                name: "InOutDetail");

            migrationBuilder.DropTable(
                name: "InventoryCheckDetail");

            migrationBuilder.DropTable(
                name: "CellBatch");

            migrationBuilder.DropTable(
                name: "InventoryCheck");

            migrationBuilder.DropIndex(
                name: "IX_ErrorTicket_InventoryCheckId",
                table: "ErrorTicket");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ErrorTicket");

            migrationBuilder.RenameColumn(
                name: "InventoryCheckId",
                table: "ErrorTicket",
                newName: "InventoryCountDetailId");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "ErrorTicket",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InventoryAdjustment",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarehouseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_InventoryAdjustment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryAdjustment_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InventoryCount",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    ScheduleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_InventoryCount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryCount_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryCount_Schedule_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedule",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InventoryAdjustmentDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NewQuantity = table.Column<float>(type: "real", nullable: false),
                    ChangeInQuantity = table.Column<float>(type: "real", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InventoryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InventoryAdjustmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_InventoryAdjustmentDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryAdjustmentDetail_InventoryAdjustment_InventoryAdjustmentId",
                        column: x => x.InventoryAdjustmentId,
                        principalTable: "InventoryAdjustment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryAdjustmentDetail_Inventory_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InventoryCountDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExpectedQuantity = table.Column<float>(type: "real", nullable: false),
                    CountedQuantity = table.Column<float>(type: "real", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InventoryCountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ErrorTicketId = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_InventoryCountDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryCountDetail_InventoryCount_InventoryCountId",
                        column: x => x.InventoryCountId,
                        principalTable: "InventoryCount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryCountDetail_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ErrorTicket_InventoryCountDetailId",
                table: "ErrorTicket",
                column: "InventoryCountDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustment_WarehouseId",
                table: "InventoryAdjustment",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustmentDetail_InventoryAdjustmentId",
                table: "InventoryAdjustmentDetail",
                column: "InventoryAdjustmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustmentDetail_InventoryId",
                table: "InventoryAdjustmentDetail",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCount_LocationId",
                table: "InventoryCount",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCount_ScheduleId",
                table: "InventoryCount",
                column: "ScheduleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCountDetail_InventoryCountId",
                table: "InventoryCountDetail",
                column: "InventoryCountId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCountDetail_ProductId",
                table: "InventoryCountDetail",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ErrorTicket_InventoryCountDetail_InventoryCountDetailId",
                table: "ErrorTicket",
                column: "InventoryCountDetailId",
                principalTable: "InventoryCountDetail",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ErrorTicket_InventoryCountDetail_InventoryCountDetailId",
                table: "ErrorTicket");

            migrationBuilder.DropTable(
                name: "InventoryAdjustmentDetail");

            migrationBuilder.DropTable(
                name: "InventoryCountDetail");

            migrationBuilder.DropTable(
                name: "InventoryAdjustment");

            migrationBuilder.DropTable(
                name: "InventoryCount");

            migrationBuilder.DropIndex(
                name: "IX_ErrorTicket_InventoryCountDetailId",
                table: "ErrorTicket");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "ErrorTicket");

            migrationBuilder.RenameColumn(
                name: "InventoryCountDetailId",
                table: "ErrorTicket",
                newName: "InventoryCheckId");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "ErrorTicket",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CellBatch",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CellId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UnitId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentStock = table.Column<int>(type: "int", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CellBatch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CellBatch_Cell_CellId",
                        column: x => x.CellId,
                        principalTable: "Cell",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CellBatch_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InventoryCheck",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ScheduleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryCheck", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryCheck_Schedule_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedule",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InOutDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BatchId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CellBatchId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    Stock = table.Column<int>(type: "int", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "InventoryCheckDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CellBatchId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InventoryCheckId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CheckedQuantity = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryCheckDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryCheckDetail_CellBatch_CellBatchId",
                        column: x => x.CellBatchId,
                        principalTable: "CellBatch",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryCheckDetail_InventoryCheck_InventoryCheckId",
                        column: x => x.InventoryCheckId,
                        principalTable: "InventoryCheck",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ErrorTicket_InventoryCheckId",
                table: "ErrorTicket",
                column: "InventoryCheckId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CellBatch_CellId",
                table: "CellBatch",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "IX_CellBatch_UnitId",
                table: "CellBatch",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_InOutDetail_BatchId",
                table: "InOutDetail",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_InOutDetail_CellBatchId",
                table: "InOutDetail",
                column: "CellBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCheck_ScheduleId",
                table: "InventoryCheck",
                column: "ScheduleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCheckDetail_CellBatchId",
                table: "InventoryCheckDetail",
                column: "CellBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCheckDetail_InventoryCheckId",
                table: "InventoryCheckDetail",
                column: "InventoryCheckId");

            migrationBuilder.AddForeignKey(
                name: "FK_ErrorTicket_InventoryCheck_InventoryCheckId",
                table: "ErrorTicket",
                column: "InventoryCheckId",
                principalTable: "InventoryCheck",
                principalColumn: "Id");
        }
    }
}
