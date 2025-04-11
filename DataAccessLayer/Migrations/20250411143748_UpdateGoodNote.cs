using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGoodNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GoodNote_GoodRequestId",
                table: "GoodNote");

            migrationBuilder.CreateIndex(
                name: "IX_GoodNote_GoodRequestId",
                table: "GoodNote",
                column: "GoodRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GoodNote_GoodRequestId",
                table: "GoodNote");

            migrationBuilder.CreateIndex(
                name: "IX_GoodNote_GoodRequestId",
                table: "GoodNote",
                column: "GoodRequestId",
                unique: true);
        }
    }
}
