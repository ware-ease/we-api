using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDBXIV : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "LocationLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "ArrangedQuantity",
                table: "Inventory",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "NotArrgangedQuantity",
                table: "Inventory",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "LocationLog");

            migrationBuilder.DropColumn(
                name: "ArrangedQuantity",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "NotArrgangedQuantity",
                table: "Inventory");
        }
    }
}
