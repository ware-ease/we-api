using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDBIV : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Area_AreaId",
                table: "Schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleSetting_Area_AreaId",
                table: "ScheduleSetting");

            migrationBuilder.DropColumn(
                name: "Day",
                table: "ScheduleSetting");

            migrationBuilder.RenameColumn(
                name: "AreaId",
                table: "ScheduleSetting",
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleSetting_AreaId",
                table: "ScheduleSetting",
                newName: "IX_ScheduleSetting_LocationId");

            migrationBuilder.RenameColumn(
                name: "AreaId",
                table: "Schedule",
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedule_AreaId",
                table: "Schedule",
                newName: "IX_Schedule_LocationId");

            migrationBuilder.AddColumn<string>(
                name: "DaysOfWeek",
                table: "ScheduleSetting",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarehouseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
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
                    table.PrimaryKey("PK_Location", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Location_Location_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Location",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Location_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Location_ParentId",
                table: "Location",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Location_WarehouseId",
                table: "Location",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Location_LocationId",
                table: "Schedule",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleSetting_Location_LocationId",
                table: "ScheduleSetting",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Location_LocationId",
                table: "Schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleSetting_Location_LocationId",
                table: "ScheduleSetting");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropColumn(
                name: "DaysOfWeek",
                table: "ScheduleSetting");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "ScheduleSetting",
                newName: "AreaId");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleSetting_LocationId",
                table: "ScheduleSetting",
                newName: "IX_ScheduleSetting_AreaId");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "Schedule",
                newName: "AreaId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedule_LocationId",
                table: "Schedule",
                newName: "IX_Schedule_AreaId");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Day",
                table: "ScheduleSetting",
                type: "date",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Area_AreaId",
                table: "Schedule",
                column: "AreaId",
                principalTable: "Area",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleSetting_Area_AreaId",
                table: "ScheduleSetting",
                column: "AreaId",
                principalTable: "Area",
                principalColumn: "Id");
        }
    }
}
