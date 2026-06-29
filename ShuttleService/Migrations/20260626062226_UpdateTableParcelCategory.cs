using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class UpdateTableParcelCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InsertBy",
                table: "ParcelCategories",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertDate",
                table: "ParcelCategories",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ParcelCategories",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "ParcelCategories",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(2026, 6, 26, 14, 22, 26, 578, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(2026, 6, 26, 14, 22, 26, 579, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsertBy",
                table: "ParcelCategories");

            migrationBuilder.DropColumn(
                name: "InsertDate",
                table: "ParcelCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ParcelCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "ParcelCategories");

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(2026, 6, 26, 10, 30, 15, 901, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(2026, 6, 26, 10, 30, 15, 901, DateTimeKind.Local));
        }
    }
}
