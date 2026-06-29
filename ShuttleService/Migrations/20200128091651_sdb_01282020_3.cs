using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_01282020_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaintananceControlNo",
                table: "MaintenanceLogs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaintananceRemarks",
                table: "MaintenanceLogs",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MaintenanceFrom",
                table: "MaintenanceLogs",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MaintenanceTo",
                table: "MaintenanceLogs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaintananceControlNo",
                table: "MaintenanceLogs");

            migrationBuilder.DropColumn(
                name: "MaintananceRemarks",
                table: "MaintenanceLogs");

            migrationBuilder.DropColumn(
                name: "MaintenanceFrom",
                table: "MaintenanceLogs");

            migrationBuilder.DropColumn(
                name: "MaintenanceTo",
                table: "MaintenanceLogs");
        }
    }
}
