using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_01282020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "ShuttlePassengers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "ShuttlePassengers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "DriverPassengers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "DriverPassengers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MaintenanceLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProcessedBy = table.Column<string>(nullable: true),
                    VehicleListId = table.Column<int>(nullable: false),
                    Process = table.Column<string>(nullable: true),
                    ProcessedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceLogs_VehicleLists_VehicleListId",
                        column: x => x.VehicleListId,
                        principalTable: "VehicleLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceLogs_VehicleListId",
                table: "MaintenanceLogs",
                column: "VehicleListId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaintenanceLogs");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "ShuttlePassengers");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "ShuttlePassengers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "DriverPassengers");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "DriverPassengers");
        }
    }
}
