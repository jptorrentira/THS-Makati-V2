using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class insertParcel231_10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "VehicleListId",
                table: "ParcelDeliveries",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "DriverId",
                table: "ParcelDeliveries",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(2025, 10, 14, 10, 14, 56, 551, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(2025, 10, 14, 10, 14, 56, 552, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "VehicleListId",
                table: "ParcelDeliveries",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DriverId",
                table: "ParcelDeliveries",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(2025, 10, 14, 10, 14, 13, 589, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(2025, 10, 14, 10, 14, 13, 590, DateTimeKind.Local));
        }
    }
}
