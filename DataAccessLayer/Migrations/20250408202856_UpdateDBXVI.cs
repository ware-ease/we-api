using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDBXVI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCountDetail_Product_ProductId",
                table: "InventoryCountDetail");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "InventoryCountDetail",
                newName: "InventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryCountDetail_ProductId",
                table: "InventoryCountDetail",
                newName: "IX_InventoryCountDetail_InventoryId");

            migrationBuilder.AddColumn<int>(
                name: "DocumentType",
                table: "InventoryAdjustment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryCountDetail_Inventory_InventoryId",
                table: "InventoryCountDetail",
                column: "InventoryId",
                principalTable: "Inventory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCountDetail_Inventory_InventoryId",
                table: "InventoryCountDetail");

            migrationBuilder.DropColumn(
                name: "DocumentType",
                table: "InventoryAdjustment");

            migrationBuilder.RenameColumn(
                name: "InventoryId",
                table: "InventoryCountDetail",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryCountDetail_InventoryId",
                table: "InventoryCountDetail",
                newName: "IX_InventoryCountDetail_ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryCountDetail_Product_ProductId",
                table: "InventoryCountDetail",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id");
        }
    }
}
