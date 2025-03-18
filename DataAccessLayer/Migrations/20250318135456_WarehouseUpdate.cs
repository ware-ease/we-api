using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class WarehouseUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Length",
                table: "Warehouse");

            migrationBuilder.RenameColumn(
                name: "Width",
                table: "Warehouse",
                newName: "Area");

            migrationBuilder.AddColumn<DateTime>(
                name: "OperateFrom",
                table: "Warehouse",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperateFrom",
                table: "Warehouse");

            migrationBuilder.RenameColumn(
                name: "Area",
                table: "Warehouse",
                newName: "Width");

            migrationBuilder.AddColumn<float>(
                name: "Length",
                table: "Warehouse",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
