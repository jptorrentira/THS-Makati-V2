using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class OriginDestination : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "DriverPassengers",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "DriverPassengers",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "DriverPassengers",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(2025, 10, 2, 13, 47, 45, 93, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(2025, 10, 2, 13, 47, 45, 94, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "DriverPassengers",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "DriverPassengers",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "DriverPassengers",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(2025, 10, 1, 11, 28, 57, 968, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(2025, 10, 1, 11, 28, 57, 969, DateTimeKind.Local));
        }
    }
}
