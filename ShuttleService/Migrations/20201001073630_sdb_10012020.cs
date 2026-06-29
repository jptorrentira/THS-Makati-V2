using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_10012020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeclinedBy",
                table: "ShuttlePassengers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeclinedDatetime",
                table: "ShuttlePassengers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeclinedReason",
                table: "ShuttlePassengers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeclinedBy",
                table: "DriverPassengers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeclinedDatetime",
                table: "DriverPassengers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeclinedReason",
                table: "DriverPassengers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeclinedBy",
                table: "ShuttlePassengers");

            migrationBuilder.DropColumn(
                name: "DeclinedDatetime",
                table: "ShuttlePassengers");

            migrationBuilder.DropColumn(
                name: "DeclinedReason",
                table: "ShuttlePassengers");

            migrationBuilder.DropColumn(
                name: "DeclinedBy",
                table: "DriverPassengers");

            migrationBuilder.DropColumn(
                name: "DeclinedDatetime",
                table: "DriverPassengers");

            migrationBuilder.DropColumn(
                name: "DeclinedReason",
                table: "DriverPassengers");
        }
    }
}
