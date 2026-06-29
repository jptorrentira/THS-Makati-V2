using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class OriginDestination1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(2025, 10, 2, 14, 14, 58, 108, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(2025, 10, 2, 14, 14, 58, 109, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
