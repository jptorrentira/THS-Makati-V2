using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class _010520261 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DestinationTag",
                table: "DriverPassengerHeaders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginTag",
                table: "DriverPassengerHeaders",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(2026, 1, 5, 13, 28, 52, 229, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(2026, 1, 5, 13, 28, 52, 229, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationTag",
                table: "DriverPassengerHeaders");

            migrationBuilder.DropColumn(
                name: "OriginTag",
                table: "DriverPassengerHeaders");

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(2026, 1, 5, 11, 58, 12, 652, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(2026, 1, 5, 11, 58, 12, 653, DateTimeKind.Local));
        }
    }
}
