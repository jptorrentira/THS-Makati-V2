using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_01242020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RemovedBy",
                table: "ShuttlePassengers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RemovedDatetime",
                table: "ShuttlePassengers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RemovedRemarks",
                table: "ShuttlePassengers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReservedBy",
                table: "ShuttlePassengers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReservedDatetime",
                table: "ShuttlePassengers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RemovedBy",
                table: "DriverPassengers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RemovedDatetime",
                table: "DriverPassengers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RemovedRemarks",
                table: "DriverPassengers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReservedBy",
                table: "DriverPassengers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReservedDatetime",
                table: "DriverPassengers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemovedBy",
                table: "ShuttlePassengers");

            migrationBuilder.DropColumn(
                name: "RemovedDatetime",
                table: "ShuttlePassengers");

            migrationBuilder.DropColumn(
                name: "RemovedRemarks",
                table: "ShuttlePassengers");

            migrationBuilder.DropColumn(
                name: "ReservedBy",
                table: "ShuttlePassengers");

            migrationBuilder.DropColumn(
                name: "ReservedDatetime",
                table: "ShuttlePassengers");

            migrationBuilder.DropColumn(
                name: "RemovedBy",
                table: "DriverPassengers");

            migrationBuilder.DropColumn(
                name: "RemovedDatetime",
                table: "DriverPassengers");

            migrationBuilder.DropColumn(
                name: "RemovedRemarks",
                table: "DriverPassengers");

            migrationBuilder.DropColumn(
                name: "ReservedBy",
                table: "DriverPassengers");

            migrationBuilder.DropColumn(
                name: "ReservedDatetime",
                table: "DriverPassengers");
        }
    }
}
