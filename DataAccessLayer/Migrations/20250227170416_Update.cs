using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NoteDetail_ProductType_ProductTypeId",
                table: "NoteDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDetail_ProductType_ProductTypeId",
                table: "PurchaseDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptDetail_ProductType_ProductTypeId",
                table: "ReceiptDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptDetail_ReceivingNote_NoteId",
                table: "ReceiptDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Shelf_Warehouse_WarehouseId",
                table: "Shelf");

            migrationBuilder.DropForeignKey(
                name: "FK_StockCardDetail_ProductType_ProductTypeId",
                table: "StockCardDetail");

            migrationBuilder.DropTable(
                name: "ProductTypeTypeDetails");

            migrationBuilder.DropTable(
                name: "SaleDetail");

            migrationBuilder.DropTable(
                name: "ProductType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StockCardDetail",
                table: "StockCardDetail");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptDetail_NoteId",
                table: "ReceiptDetail");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseDetail_ProductTypeId",
                table: "PurchaseDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountWarehouse",
                table: "AccountWarehouse");

            migrationBuilder.DropColumn(
                name: "In",
                table: "StockCardDetail");

            migrationBuilder.DropColumn(
                name: "Out",
                table: "StockCardDetail");

            migrationBuilder.DropColumn(
                name: "ProductTypeId",
                table: "PurchaseDetail");

            migrationBuilder.RenameColumn(
                name: "ProductTypeId",
                table: "StockCardDetail",
                newName: "BatchId");

            migrationBuilder.RenameIndex(
                name: "IX_StockCardDetail_ProductTypeId",
                table: "StockCardDetail",
                newName: "IX_StockCardDetail_BatchId");

            migrationBuilder.RenameColumn(
                name: "WarehouseId",
                table: "Shelf",
                newName: "AreaId");

            migrationBuilder.RenameIndex(
                name: "IX_Shelf_WarehouseId",
                table: "Shelf",
                newName: "IX_Shelf_AreaId");

            migrationBuilder.RenameColumn(
                name: "ProductTypeId",
                table: "ReceiptDetail",
                newName: "BrandId");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptDetail_ProductTypeId",
                table: "ReceiptDetail",
                newName: "IX_ReceiptDetail_BrandId");

            migrationBuilder.RenameColumn(
                name: "ProductTypeId",
                table: "NoteDetail",
                newName: "BatchId");

            migrationBuilder.RenameIndex(
                name: "IX_NoteDetail_ProductTypeId",
                table: "NoteDetail",
                newName: "IX_NoteDetail_BatchId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TypeDetail",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TypeDetail",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "StockCardDetail",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "StockCardDetail",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "StockCardDetail",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "ReceivingNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "ReceivingNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverName",
                table: "ReceivingNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShpperName",
                table: "ReceivingNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "NoteId",
                table: "ReceiptDetail",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "receivingNoteId",
                table: "ReceiptDetail",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BarCode",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BrandId",
                table: "Product",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InventoryId",
                table: "Product",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sku",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UnitId",
                table: "Product",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "IssueNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "IssueNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IssuerName",
                table: "IssueNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "IssueNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverName",
                table: "IssueNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Customer",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Category",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AccountWarehouse",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockCardDetail",
                table: "StockCardDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountWarehouse",
                table: "AccountWarehouse",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Batch",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SupplierId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MfgDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: true),
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
                    table.PrimaryKey("PK_Batch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Batch_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Brand",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Brand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    MaxQuantity = table.Column<int>(type: "int", nullable: false),
                    MinQuantity = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Inventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventory_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Inventory_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Unit",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Unit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Area",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FloorNumber = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ScheduleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ScheduleSettingId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_Area", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Area_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScheduleSetting",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Day = table.Column<DateOnly>(type: "date", nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    AreaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_ScheduleSetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleSetting_Area_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Area",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ErrorTicket",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HandelBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InventoryCheckId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_ErrorTicket", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryCheck",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    ScheduleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ErrorTicketId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InventoryCheckDetailId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StockCardId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_InventoryCheck", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryCheck_ErrorTicket_ErrorTicketId",
                        column: x => x.ErrorTicketId,
                        principalTable: "ErrorTicket",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryCheck_StockCard_StockCardId",
                        column: x => x.StockCardId,
                        principalTable: "StockCard",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InventoryCheckDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CheckedQuantity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InventoryCheckId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StockCardId = table.Column<string>(type: "nvarchar(450)", nullable: true),
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
                    table.PrimaryKey("PK_InventoryCheckDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryCheckDetail_InventoryCheck_InventoryCheckId",
                        column: x => x.InventoryCheckId,
                        principalTable: "InventoryCheck",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryCheckDetail_StockCard_StockCardId",
                        column: x => x.StockCardId,
                        principalTable: "StockCard",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    AreaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InventoryCheckId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_Schedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedule_Area_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Area",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Schedule_InventoryCheck_InventoryCheckId",
                        column: x => x.InventoryCheckId,
                        principalTable: "InventoryCheck",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockCardDetail_StockCardId",
                table: "StockCardDetail",
                column: "StockCardId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDetail_receivingNoteId",
                table: "ReceiptDetail",
                column: "receivingNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_BrandId",
                table: "Product",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_InventoryId",
                table: "Product",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_UnitId",
                table: "Product",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountWarehouse_AccountId",
                table: "AccountWarehouse",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Area_ScheduleId",
                table: "Area",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Area_ScheduleSettingId",
                table: "Area",
                column: "ScheduleSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_Area_WarehouseId",
                table: "Area",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Batch_ProductId",
                table: "Batch",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorTicket_InventoryCheckId",
                table: "ErrorTicket",
                column: "InventoryCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_ProductId",
                table: "Inventory",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_WarehouseId",
                table: "Inventory",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCheck_ErrorTicketId",
                table: "InventoryCheck",
                column: "ErrorTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCheck_InventoryCheckDetailId",
                table: "InventoryCheck",
                column: "InventoryCheckDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCheck_ScheduleId",
                table: "InventoryCheck",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCheck_StockCardId",
                table: "InventoryCheck",
                column: "StockCardId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCheckDetail_InventoryCheckId",
                table: "InventoryCheckDetail",
                column: "InventoryCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCheckDetail_StockCardId",
                table: "InventoryCheckDetail",
                column: "StockCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_AreaId",
                table: "Schedule",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_InventoryCheckId",
                table: "Schedule",
                column: "InventoryCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleSetting_AreaId",
                table: "ScheduleSetting",
                column: "AreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_NoteDetail_Batch_BatchId",
                table: "NoteDetail",
                column: "BatchId",
                principalTable: "Batch",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Brand_BrandId",
                table: "Product",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Inventory_InventoryId",
                table: "Product",
                column: "InventoryId",
                principalTable: "Inventory",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Unit_UnitId",
                table: "Product",
                column: "UnitId",
                principalTable: "Unit",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptDetail_Brand_BrandId",
                table: "ReceiptDetail",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptDetail_ReceivingNote_receivingNoteId",
                table: "ReceiptDetail",
                column: "receivingNoteId",
                principalTable: "ReceivingNote",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shelf_Area_AreaId",
                table: "Shelf",
                column: "AreaId",
                principalTable: "Area",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockCardDetail_Batch_BatchId",
                table: "StockCardDetail",
                column: "BatchId",
                principalTable: "Batch",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Area_ScheduleSetting_ScheduleSettingId",
                table: "Area",
                column: "ScheduleSettingId",
                principalTable: "ScheduleSetting",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Area_Schedule_ScheduleId",
                table: "Area",
                column: "ScheduleId",
                principalTable: "Schedule",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ErrorTicket_InventoryCheck_InventoryCheckId",
                table: "ErrorTicket",
                column: "InventoryCheckId",
                principalTable: "InventoryCheck",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryCheck_InventoryCheckDetail_InventoryCheckDetailId",
                table: "InventoryCheck",
                column: "InventoryCheckDetailId",
                principalTable: "InventoryCheckDetail",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryCheck_Schedule_ScheduleId",
                table: "InventoryCheck",
                column: "ScheduleId",
                principalTable: "Schedule",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NoteDetail_Batch_BatchId",
                table: "NoteDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Brand_BrandId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Inventory_InventoryId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Unit_UnitId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptDetail_Brand_BrandId",
                table: "ReceiptDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptDetail_ReceivingNote_receivingNoteId",
                table: "ReceiptDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Shelf_Area_AreaId",
                table: "Shelf");

            migrationBuilder.DropForeignKey(
                name: "FK_StockCardDetail_Batch_BatchId",
                table: "StockCardDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Area_ScheduleSetting_ScheduleSettingId",
                table: "Area");

            migrationBuilder.DropForeignKey(
                name: "FK_Area_Schedule_ScheduleId",
                table: "Area");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCheck_Schedule_ScheduleId",
                table: "InventoryCheck");

            migrationBuilder.DropForeignKey(
                name: "FK_ErrorTicket_InventoryCheck_InventoryCheckId",
                table: "ErrorTicket");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCheckDetail_InventoryCheck_InventoryCheckId",
                table: "InventoryCheckDetail");

            migrationBuilder.DropTable(
                name: "Batch");

            migrationBuilder.DropTable(
                name: "Brand");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "Unit");

            migrationBuilder.DropTable(
                name: "ScheduleSetting");

            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.DropTable(
                name: "Area");

            migrationBuilder.DropTable(
                name: "InventoryCheck");

            migrationBuilder.DropTable(
                name: "ErrorTicket");

            migrationBuilder.DropTable(
                name: "InventoryCheckDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StockCardDetail",
                table: "StockCardDetail");

            migrationBuilder.DropIndex(
                name: "IX_StockCardDetail_StockCardId",
                table: "StockCardDetail");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptDetail_receivingNoteId",
                table: "ReceiptDetail");

            migrationBuilder.DropIndex(
                name: "IX_Product_BrandId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_InventoryId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_UnitId",
                table: "Product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountWarehouse",
                table: "AccountWarehouse");

            migrationBuilder.DropIndex(
                name: "IX_AccountWarehouse_AccountId",
                table: "AccountWarehouse");

            migrationBuilder.DropColumn(
                name: "Action",
                table: "StockCardDetail");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "StockCardDetail");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "ReceivingNote");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "ReceivingNote");

            migrationBuilder.DropColumn(
                name: "ReceiverName",
                table: "ReceivingNote");

            migrationBuilder.DropColumn(
                name: "ShpperName",
                table: "ReceivingNote");

            migrationBuilder.DropColumn(
                name: "receivingNoteId",
                table: "ReceiptDetail");

            migrationBuilder.DropColumn(
                name: "BarCode",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Sku",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "IssueNote");

            migrationBuilder.DropColumn(
                name: "Destination",
                table: "IssueNote");

            migrationBuilder.DropColumn(
                name: "IssuerName",
                table: "IssueNote");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "IssueNote");

            migrationBuilder.DropColumn(
                name: "ReceiverName",
                table: "IssueNote");

            migrationBuilder.RenameColumn(
                name: "BatchId",
                table: "StockCardDetail",
                newName: "ProductTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_StockCardDetail_BatchId",
                table: "StockCardDetail",
                newName: "IX_StockCardDetail_ProductTypeId");

            migrationBuilder.RenameColumn(
                name: "AreaId",
                table: "Shelf",
                newName: "WarehouseId");

            migrationBuilder.RenameIndex(
                name: "IX_Shelf_AreaId",
                table: "Shelf",
                newName: "IX_Shelf_WarehouseId");

            migrationBuilder.RenameColumn(
                name: "BrandId",
                table: "ReceiptDetail",
                newName: "ProductTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptDetail_BrandId",
                table: "ReceiptDetail",
                newName: "IX_ReceiptDetail_ProductTypeId");

            migrationBuilder.RenameColumn(
                name: "BatchId",
                table: "NoteDetail",
                newName: "ProductTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_NoteDetail_BatchId",
                table: "NoteDetail",
                newName: "IX_NoteDetail_ProductTypeId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TypeDetail",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TypeDetail",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "StockCardDetail",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "In",
                table: "StockCardDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Out",
                table: "StockCardDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NoteId",
                table: "ReceiptDetail",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ProductTypeId",
                table: "PurchaseDetail",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Customer",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Category",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AccountWarehouse",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Account",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockCardDetail",
                table: "StockCardDetail",
                columns: new[] { "StockCardId", "ProductTypeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountWarehouse",
                table: "AccountWarehouse",
                columns: new[] { "AccountId", "WarehouseId" });

            migrationBuilder.CreateTable(
                name: "ProductType",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductType_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductTypeTypeDetails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductTypeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TypeDetailId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypeTypeDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductTypeTypeDetails_ProductType_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalTable: "ProductType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductTypeTypeDetails_TypeDetail_TypeDetailId",
                        column: x => x.TypeDetailId,
                        principalTable: "TypeDetail",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SaleDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductTypeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReceiptId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Price = table.Column<float>(type: "real", nullable: false),
                    Quanlity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleDetail_ProductType_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalTable: "ProductType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SaleDetail_SaleReceipt_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "SaleReceipt",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDetail_NoteId",
                table: "ReceiptDetail",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDetail_ProductTypeId",
                table: "PurchaseDetail",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductType_ProductId",
                table: "ProductType",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypeTypeDetails_ProductTypeId",
                table: "ProductTypeTypeDetails",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypeTypeDetails_TypeDetailId",
                table: "ProductTypeTypeDetails",
                column: "TypeDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleDetail_ProductTypeId",
                table: "SaleDetail",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleDetail_ReceiptId",
                table: "SaleDetail",
                column: "ReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_NoteDetail_ProductType_ProductTypeId",
                table: "NoteDetail",
                column: "ProductTypeId",
                principalTable: "ProductType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDetail_ProductType_ProductTypeId",
                table: "PurchaseDetail",
                column: "ProductTypeId",
                principalTable: "ProductType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptDetail_ProductType_ProductTypeId",
                table: "ReceiptDetail",
                column: "ProductTypeId",
                principalTable: "ProductType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptDetail_ReceivingNote_NoteId",
                table: "ReceiptDetail",
                column: "NoteId",
                principalTable: "ReceivingNote",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shelf_Warehouse_WarehouseId",
                table: "Shelf",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockCardDetail_ProductType_ProductTypeId",
                table: "StockCardDetail",
                column: "ProductTypeId",
                principalTable: "ProductType",
                principalColumn: "Id");
        }
    }
}
