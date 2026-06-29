using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_01222020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "ShuttlePassengers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledBy",
                table: "ShuttlePassengers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledDatetime",
                table: "ShuttlePassengers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "DriverPassengers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledBy",
                table: "DriverPassengers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledDatetime",
                table: "DriverPassengers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "ShuttlePassengers");

            migrationBuilder.DropColumn(
                name: "CancelledBy",
                table: "ShuttlePassengers");

            migrationBuilder.DropColumn(
                name: "CancelledDatetime",
                table: "ShuttlePassengers");

            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "DriverPassengers");

            migrationBuilder.DropColumn(
                name: "CancelledBy",
                table: "DriverPassengers");

            migrationBuilder.DropColumn(
                name: "CancelledDatetime",
                table: "DriverPassengers");
        }
    }
}
