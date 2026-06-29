using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class insertParcel231_9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryRemarks",
                table: "ParcelDeliveries",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DriverId",
                table: "ParcelDeliveries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VehicleListId",
                table: "ParcelDeliveries",
                nullable: false,
                defaultValue: 0);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryRemarks",
                table: "ParcelDeliveries");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "ParcelDeliveries");

            migrationBuilder.DropColumn(
                name: "VehicleListId",
                table: "ParcelDeliveries");

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(2025, 10, 13, 16, 33, 23, 844, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(2025, 10, 13, 16, 33, 23, 844, DateTimeKind.Local));
        }
    }
}
