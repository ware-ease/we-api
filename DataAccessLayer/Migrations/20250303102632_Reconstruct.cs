using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Reconstruct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Area_ScheduleSetting_ScheduleSettingId",
                table: "Area");

            migrationBuilder.DropForeignKey(
                name: "FK_Area_Schedule_ScheduleId",
                table: "Area");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCheck_CellBatch_StockCardId",
                table: "InventoryCheck");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCheck_ErrorTicket_ErrorTicketId",
                table: "InventoryCheck");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCheck_InventoryCheckDetail_InventoryCheckDetailId",
                table: "InventoryCheck");

            migrationBuilder.DropForeignKey(
                name: "FK_IssueNote_SaleReceipt_SaleReceiptId",
                table: "IssueNote");

            migrationBuilder.DropForeignKey(
                name: "FK_NoteDetail_Batch_BatchId",
                table: "NoteDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_NoteDetail_IssueNote_NoteId",
                table: "NoteDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Inventory_InventoryId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptDetail_Batch_BatchId",
                table: "ReceiptDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptDetail_ReceivingNote_receivingNoteId",
                table: "ReceiptDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceivingNote_PurchaseReceipt_PurchaseId",
                table: "ReceivingNote");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_InventoryCheck_InventoryCheckId",
                table: "Schedule");

            migrationBuilder.DropTable(
                name: "PurchaseDetail");

            migrationBuilder.DropTable(
                name: "SaleReceipt");

            migrationBuilder.DropTable(
                name: "TypeDetail");

            migrationBuilder.DropTable(
                name: "PurchaseReceipt");

            migrationBuilder.DropIndex(
                name: "IX_Schedule_InventoryCheckId",
                table: "Schedule");

            migrationBuilder.DropIndex(
                name: "IX_ReceivingNote_PurchaseId",
                table: "ReceivingNote");

            migrationBuilder.DropIndex(
                name: "IX_Profile_AccountId",
                table: "Profile");

            migrationBuilder.DropIndex(
                name: "IX_Product_InventoryId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_IssueNote_SaleReceiptId",
                table: "IssueNote");

            migrationBuilder.DropIndex(
                name: "IX_InventoryCheck_ErrorTicketId",
                table: "InventoryCheck");

            migrationBuilder.DropIndex(
                name: "IX_InventoryCheck_InventoryCheckDetailId",
                table: "InventoryCheck");

            migrationBuilder.DropIndex(
                name: "IX_InventoryCheck_ScheduleId",
                table: "InventoryCheck");

            migrationBuilder.DropIndex(
                name: "IX_InventoryCheck_StockCardId",
                table: "InventoryCheck");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupAction",
                table: "GroupAction");

            migrationBuilder.DropIndex(
                name: "IX_ErrorTicket_InventoryCheckId",
                table: "ErrorTicket");

            migrationBuilder.DropIndex(
                name: "IX_Area_ScheduleId",
                table: "Area");

            migrationBuilder.DropIndex(
                name: "IX_Area_ScheduleSettingId",
                table: "Area");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountGroup",
                table: "AccountGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReceiptDetail",
                table: "ReceiptDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NoteDetail",
                table: "NoteDetail");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Warehouse");

            migrationBuilder.DropColumn(
                name: "FloorNumber",
                table: "Shelf");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Shelf");

            migrationBuilder.DropColumn(
                name: "InventoryCheckId",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "PurchaseId",
                table: "ReceivingNote");

            migrationBuilder.DropColumn(
                name: "ShpperName",
                table: "ReceivingNote");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "SaleReceiptId",
                table: "IssueNote");

            migrationBuilder.DropColumn(
                name: "ErrorTicketId",
                table: "InventoryCheck");

            migrationBuilder.DropColumn(
                name: "InventoryCheckDetailId",
                table: "InventoryCheck");

            migrationBuilder.DropColumn(
                name: "StockCardId",
                table: "InventoryCheck");

            migrationBuilder.DropColumn(
                name: "HandelBy",
                table: "ErrorTicket");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "CellBatch");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CellBatch");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "CellBatch");

            migrationBuilder.DropColumn(
                name: "FloorNumber",
                table: "Area");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Area");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Area");

            migrationBuilder.DropColumn(
                name: "ScheduleSettingId",
                table: "Area");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "ReceiptDetail");

            migrationBuilder.DropColumn(
                name: "Quanlity",
                table: "ReceiptDetail");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "NoteDetail");

            migrationBuilder.DropColumn(
                name: "Quanlity",
                table: "NoteDetail");

            migrationBuilder.RenameTable(
                name: "ReceiptDetail",
                newName: "ReceivingDetail");

            migrationBuilder.RenameTable(
                name: "NoteDetail",
                newName: "IssueDetail");

            migrationBuilder.RenameColumn(
                name: "BarCode",
                table: "Product",
                newName: "Barcode");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptDetail_receivingNoteId",
                table: "ReceivingDetail",
                newName: "IX_ReceivingDetail_receivingNoteId");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptDetail_BatchId",
                table: "ReceivingDetail",
                newName: "IX_ReceivingDetail_BatchId");

            migrationBuilder.RenameIndex(
                name: "IX_NoteDetail_NoteId",
                table: "IssueDetail",
                newName: "IX_IssueDetail_NoteId");

            migrationBuilder.RenameIndex(
                name: "IX_NoteDetail_BatchId",
                table: "IssueDetail",
                newName: "IX_IssueDetail_BatchId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Warehouse",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Warehouse",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Unit",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Supplier",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Supplier",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "StockBook",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Shelf",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverName",
                table: "ReceivingNote",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "ReceivingNote",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "ReceivingNote",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ShipperName",
                table: "ReceivingNote",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WarehouseId",
                table: "ReceivingNote",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nationality",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "Profile",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Sku",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Barcode",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Permission",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Permission",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverName",
                table: "IssueNote",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "IssueNote",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "IssuerName",
                table: "IssueNote",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Destination",
                table: "IssueNote",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "IssueNote",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "IssueNote",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "WarehouseId",
                table: "IssueNote",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "InventoryCheckDetail",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "CheckedQuantity",
                table: "InventoryCheckDetail",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "InventoryCheck",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MinQuantity",
                table: "Inventory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MaxQuantity",
                table: "Inventory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Stock",
                table: "InOutDetail",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "InOutDetail",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CellBatchId",
                table: "InOutDetail",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "InOutDetail",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "GroupAction",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Group",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "ErrorTicket",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "ErrorTicket",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "HandleBy",
                table: "ErrorTicket",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "CurrentStock",
                table: "CellBatch",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UnitId",
                table: "CellBatch",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Category",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SupplierId",
                table: "Batch",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Quantity",
                table: "Batch",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ProductId",
                table: "Batch",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Batch",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Batch",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Area",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Action",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AccountGroup",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupAction",
                table: "GroupAction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountGroup",
                table: "AccountGroup",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReceivingDetail",
                table: "ReceivingDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IssueDetail",
                table: "IssueDetail",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivingNote_WarehouseId",
                table: "ReceivingNote",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Profile_AccountId",
                table: "Profile",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IssueNote_WarehouseId",
                table: "IssueNote",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCheck_ScheduleId",
                table: "InventoryCheck",
                column: "ScheduleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupAction_GroupId",
                table: "GroupAction",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorTicket_InventoryCheckId",
                table: "ErrorTicket",
                column: "InventoryCheckId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CellBatch_UnitId",
                table: "CellBatch",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountGroup_AccountId",
                table: "AccountGroup",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_CellBatch_Unit_UnitId",
                table: "CellBatch",
                column: "UnitId",
                principalTable: "Unit",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueDetail_Batch_BatchId",
                table: "IssueDetail",
                column: "BatchId",
                principalTable: "Batch",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueDetail_IssueNote_NoteId",
                table: "IssueDetail",
                column: "NoteId",
                principalTable: "IssueNote",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueNote_Warehouse_WarehouseId",
                table: "IssueNote",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivingDetail_Batch_BatchId",
                table: "ReceivingDetail",
                column: "BatchId",
                principalTable: "Batch",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivingDetail_ReceivingNote_receivingNoteId",
                table: "ReceivingDetail",
                column: "receivingNoteId",
                principalTable: "ReceivingNote",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivingNote_Warehouse_WarehouseId",
                table: "ReceivingNote",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CellBatch_Unit_UnitId",
                table: "CellBatch");

            migrationBuilder.DropForeignKey(
                name: "FK_IssueDetail_Batch_BatchId",
                table: "IssueDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_IssueDetail_IssueNote_NoteId",
                table: "IssueDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_IssueNote_Warehouse_WarehouseId",
                table: "IssueNote");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceivingDetail_Batch_BatchId",
                table: "ReceivingDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceivingDetail_ReceivingNote_receivingNoteId",
                table: "ReceivingDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceivingNote_Warehouse_WarehouseId",
                table: "ReceivingNote");

            migrationBuilder.DropIndex(
                name: "IX_ReceivingNote_WarehouseId",
                table: "ReceivingNote");

            migrationBuilder.DropIndex(
                name: "IX_Profile_AccountId",
                table: "Profile");

            migrationBuilder.DropIndex(
                name: "IX_IssueNote_WarehouseId",
                table: "IssueNote");

            migrationBuilder.DropIndex(
                name: "IX_InventoryCheck_ScheduleId",
                table: "InventoryCheck");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupAction",
                table: "GroupAction");

            migrationBuilder.DropIndex(
                name: "IX_GroupAction_GroupId",
                table: "GroupAction");

            migrationBuilder.DropIndex(
                name: "IX_ErrorTicket_InventoryCheckId",
                table: "ErrorTicket");

            migrationBuilder.DropIndex(
                name: "IX_CellBatch_UnitId",
                table: "CellBatch");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountGroup",
                table: "AccountGroup");

            migrationBuilder.DropIndex(
                name: "IX_AccountGroup_AccountId",
                table: "AccountGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReceivingDetail",
                table: "ReceivingDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IssueDetail",
                table: "IssueDetail");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Shelf");

            migrationBuilder.DropColumn(
                name: "ShipperName",
                table: "ReceivingNote");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "ReceivingNote");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "IssueNote");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "InOutDetail");

            migrationBuilder.DropColumn(
                name: "HandleBy",
                table: "ErrorTicket");

            migrationBuilder.DropColumn(
                name: "CurrentStock",
                table: "CellBatch");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "CellBatch");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Area");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Account");

            migrationBuilder.RenameTable(
                name: "ReceivingDetail",
                newName: "ReceiptDetail");

            migrationBuilder.RenameTable(
                name: "IssueDetail",
                newName: "NoteDetail");

            migrationBuilder.RenameColumn(
                name: "Barcode",
                table: "Product",
                newName: "BarCode");

            migrationBuilder.RenameIndex(
                name: "IX_ReceivingDetail_receivingNoteId",
                table: "ReceiptDetail",
                newName: "IX_ReceiptDetail_receivingNoteId");

            migrationBuilder.RenameIndex(
                name: "IX_ReceivingDetail_BatchId",
                table: "ReceiptDetail",
                newName: "IX_ReceiptDetail_BatchId");

            migrationBuilder.RenameIndex(
                name: "IX_IssueDetail_NoteId",
                table: "NoteDetail",
                newName: "IX_NoteDetail_NoteId");

            migrationBuilder.RenameIndex(
                name: "IX_IssueDetail_BatchId",
                table: "NoteDetail",
                newName: "IX_NoteDetail_BatchId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Warehouse",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Warehouse",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentId",
                table: "Warehouse",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Unit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Supplier",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Supplier",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "StockBook",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FloorNumber",
                table: "Shelf",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Shelf",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InventoryCheckId",
                table: "Schedule",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverName",
                table: "ReceivingNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "ReceivingNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "ReceivingNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PurchaseId",
                table: "ReceivingNote",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShpperName",
                table: "ReceivingNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nationality",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Profile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "Profile",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Sku",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BarCode",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InventoryId",
                table: "Product",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Permission",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Permission",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverName",
                table: "IssueNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "IssueNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IssuerName",
                table: "IssueNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Destination",
                table: "IssueNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "IssueNote",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "IssueNote",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SaleReceiptId",
                table: "IssueNote",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "InventoryCheckDetail",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CheckedQuantity",
                table: "InventoryCheckDetail",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "InventoryCheck",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "ErrorTicketId",
                table: "InventoryCheck",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InventoryCheckDetailId",
                table: "InventoryCheck",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StockCardId",
                table: "InventoryCheck",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "MinQuantity",
                table: "Inventory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaxQuantity",
                table: "Inventory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Stock",
                table: "InOutDetail",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "InOutDetail",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CellBatchId",
                table: "InOutDetail",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "GroupAction",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Group",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "ErrorTicket",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "ErrorTicket",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HandelBy",
                table: "ErrorTicket",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "CellBatch",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CellBatch",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "CellBatch",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Category",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SupplierId",
                table: "Batch",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Quantity",
                table: "Batch",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductId",
                table: "Batch",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Batch",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Batch",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FloorNumber",
                table: "Area",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "Area",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ScheduleId",
                table: "Area",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ScheduleSettingId",
                table: "Area",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Action",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AccountGroup",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<float>(
                name: "Price",
                table: "ReceiptDetail",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "Quanlity",
                table: "ReceiptDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "Price",
                table: "NoteDetail",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "Quanlity",
                table: "NoteDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupAction",
                table: "GroupAction",
                columns: new[] { "GroupId", "ActionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountGroup",
                table: "AccountGroup",
                columns: new[] { "AccountId", "GroupId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReceiptDetail",
                table: "ReceiptDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NoteDetail",
                table: "NoteDetail",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PurchaseReceipt",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SupplierId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseReceipt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseReceipt_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SaleReceipt",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleReceipt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleReceipt_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TypeDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeDetail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_PurchaseDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseDetail_PurchaseReceipt_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "PurchaseReceipt",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_InventoryCheckId",
                table: "Schedule",
                column: "InventoryCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivingNote_PurchaseId",
                table: "ReceivingNote",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Profile_AccountId",
                table: "Profile",
                column: "AccountId",
                unique: true,
                filter: "[AccountId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Product_InventoryId",
                table: "Product",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueNote_SaleReceiptId",
                table: "IssueNote",
                column: "SaleReceiptId");

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
                name: "IX_ErrorTicket_InventoryCheckId",
                table: "ErrorTicket",
                column: "InventoryCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_Area_ScheduleId",
                table: "Area",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Area_ScheduleSettingId",
                table: "Area",
                column: "ScheduleSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDetail_ReceiptId",
                table: "PurchaseDetail",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReceipt_SupplierId",
                table: "PurchaseReceipt",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReceipt_CustomerId",
                table: "SaleReceipt",
                column: "CustomerId");

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
                name: "FK_InventoryCheck_CellBatch_StockCardId",
                table: "InventoryCheck",
                column: "StockCardId",
                principalTable: "CellBatch",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryCheck_ErrorTicket_ErrorTicketId",
                table: "InventoryCheck",
                column: "ErrorTicketId",
                principalTable: "ErrorTicket",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryCheck_InventoryCheckDetail_InventoryCheckDetailId",
                table: "InventoryCheck",
                column: "InventoryCheckDetailId",
                principalTable: "InventoryCheckDetail",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueNote_SaleReceipt_SaleReceiptId",
                table: "IssueNote",
                column: "SaleReceiptId",
                principalTable: "SaleReceipt",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NoteDetail_Batch_BatchId",
                table: "NoteDetail",
                column: "BatchId",
                principalTable: "Batch",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NoteDetail_IssueNote_NoteId",
                table: "NoteDetail",
                column: "NoteId",
                principalTable: "IssueNote",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Inventory_InventoryId",
                table: "Product",
                column: "InventoryId",
                principalTable: "Inventory",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptDetail_Batch_BatchId",
                table: "ReceiptDetail",
                column: "BatchId",
                principalTable: "Batch",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptDetail_ReceivingNote_receivingNoteId",
                table: "ReceiptDetail",
                column: "receivingNoteId",
                principalTable: "ReceivingNote",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivingNote_PurchaseReceipt_PurchaseId",
                table: "ReceivingNote",
                column: "PurchaseId",
                principalTable: "PurchaseReceipt",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_InventoryCheck_InventoryCheckId",
                table: "Schedule",
                column: "InventoryCheckId",
                principalTable: "InventoryCheck",
                principalColumn: "Id");
        }
    }
}
