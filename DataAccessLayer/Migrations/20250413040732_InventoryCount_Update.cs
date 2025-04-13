using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class InventoryCount_Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCount_Location_LocationId",
                table: "InventoryCount");

            migrationBuilder.DropIndex(
                name: "IX_InventoryCount_LocationId",
                table: "InventoryCount");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "InventoryCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocationId",
                table: "InventoryCount",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCount_LocationId",
                table: "InventoryCount",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryCount_Location_LocationId",
                table: "InventoryCount",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id");
        }
    }
}
