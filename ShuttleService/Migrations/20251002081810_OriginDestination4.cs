using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class OriginDestination4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationTag",
                table: "DriverPassengers");

            migrationBuilder.DropColumn(
                name: "OriginTag",
                table: "DriverPassengers");

            migrationBuilder.DropColumn(
                name: "DestinationTag",
                table: "DriverPassengerHeaders");

            migrationBuilder.DropColumn(
                name: "OriginTag",
                table: "DriverPassengerHeaders");

            migrationBuilder.AddColumn<int>(
                name: "DestinationId",
                table: "DriverPassengers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OriginId",
                table: "DriverPassengers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DestinationId",
                table: "DriverPassengerHeaders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OriginId",
                table: "DriverPassengerHeaders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(2025, 10, 2, 16, 18, 9, 867, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(2025, 10, 2, 16, 18, 9, 868, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationId",
                table: "DriverPassengers");

            migrationBuilder.DropColumn(
                name: "OriginId",
                table: "DriverPassengers");

            migrationBuilder.DropColumn(
                name: "DestinationId",
                table: "DriverPassengerHeaders");

            migrationBuilder.DropColumn(
                name: "OriginId",
                table: "DriverPassengerHeaders");

            migrationBuilder.AddColumn<string>(
                name: "DestinationTag",
                table: "DriverPassengers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginTag",
                table: "DriverPassengers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DestinationTag",
                table: "DriverPassengerHeaders",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginTag",
                table: "DriverPassengerHeaders",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(2025, 10, 2, 14, 20, 10, 709, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(2025, 10, 2, 14, 20, 10, 709, DateTimeKind.Local));
        }
    }
}
