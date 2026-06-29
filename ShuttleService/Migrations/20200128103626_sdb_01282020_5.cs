using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_01282020_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "MaintananceRemarks",
                table: "MaintenanceLogs",
                newName: "MaintenanceRemarks");

            migrationBuilder.RenameColumn(
                name: "MaintananceControlNo",
                table: "MaintenanceLogs",
                newName: "MaintenanceControlNo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaintenanceRemarks",
                table: "MaintenanceLogs",
                newName: "MaintananceRemarks");

            migrationBuilder.RenameColumn(
                name: "MaintenanceControlNo",
                table: "MaintenanceLogs",
                newName: "MaintananceControlNo");

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
    }
}
