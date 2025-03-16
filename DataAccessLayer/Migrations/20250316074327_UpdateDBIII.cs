using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDBIII : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Batch");

            migrationBuilder.RenameColumn(
                name: "requestType",
                table: "GoodRequest",
                newName: "RequestType");

            migrationBuilder.RenameColumn(
                name: "note",
                table: "GoodRequest",
                newName: "Note");

            migrationBuilder.AlterColumn<string>(
                name: "WarehouseId",
                table: "GoodRequest",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "RequestedWarehouseId",
                table: "GoodRequest",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "PartnerId",
                table: "GoodRequest",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "GoodNote",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NoteType = table.Column<int>(type: "int", nullable: false),
                    ShipperName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GoodRequestId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_GoodNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodNote_GoodRequest_GoodRequestId",
                        column: x => x.GoodRequestId,
                        principalTable: "GoodRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GoodRequestDetails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Quantity = table.Column<float>(type: "real", nullable: false),
                    GoodRequestId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_GoodRequestDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodRequestDetails_GoodRequest_GoodRequestId",
                        column: x => x.GoodRequestId,
                        principalTable: "GoodRequest",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GoodRequestDetails_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GoodNoteDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Quantity = table.Column<float>(type: "real", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoodNoteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_GoodNoteDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodNoteDetail_Batch_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batch",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GoodNoteDetail_GoodNote_GoodNoteId",
                        column: x => x.GoodNoteId,
                        principalTable: "GoodNote",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoodNote_GoodRequestId",
                table: "GoodNote",
                column: "GoodRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoodNoteDetail_BatchId",
                table: "GoodNoteDetail",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodNoteDetail_GoodNoteId",
                table: "GoodNoteDetail",
                column: "GoodNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodRequestDetails_GoodRequestId",
                table: "GoodRequestDetails",
                column: "GoodRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodRequestDetails_ProductId",
                table: "GoodRequestDetails",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoodNoteDetail");

            migrationBuilder.DropTable(
                name: "GoodRequestDetails");

            migrationBuilder.DropTable(
                name: "GoodNote");

            migrationBuilder.RenameColumn(
                name: "RequestType",
                table: "GoodRequest",
                newName: "requestType");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "GoodRequest",
                newName: "note");

            migrationBuilder.AlterColumn<string>(
                name: "WarehouseId",
                table: "GoodRequest",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RequestedWarehouseId",
                table: "GoodRequest",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PartnerId",
                table: "GoodRequest",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Quantity",
                table: "Batch",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
