using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCellFromProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Cell_CellId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_CellId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "CellId",
                table: "Product");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CellId",
                table: "Product",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_CellId",
                table: "Product",
                column: "CellId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Cell_CellId",
                table: "Product",
                column: "CellId",
                principalTable: "Cell",
                principalColumn: "Id");
        }
    }
}
