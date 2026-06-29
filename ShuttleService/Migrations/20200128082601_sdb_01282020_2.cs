using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_01282020_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaintananceControlNo",
                table: "VehicleLists",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaintananceRemarks",
                table: "VehicleLists",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MaintenanceFrom",
                table: "VehicleLists",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MaintenanceTo",
                table: "VehicleLists",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UnderMaintanance",
                table: "VehicleLists",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaintananceControlNo",
                table: "VehicleLists");

            migrationBuilder.DropColumn(
                name: "MaintananceRemarks",
                table: "VehicleLists");

            migrationBuilder.DropColumn(
                name: "MaintenanceFrom",
                table: "VehicleLists");

            migrationBuilder.DropColumn(
                name: "MaintenanceTo",
                table: "VehicleLists");

            migrationBuilder.DropColumn(
                name: "UnderMaintanance",
                table: "VehicleLists");
        }
    }
}
