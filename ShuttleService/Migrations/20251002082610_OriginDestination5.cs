using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class OriginDestination5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyGroupId",
                table: "OriginDestination",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(2025, 10, 2, 16, 26, 9, 972, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(2025, 10, 2, 16, 26, 9, 973, DateTimeKind.Local));

            migrationBuilder.CreateIndex(
                name: "IX_OriginDestination_CompanyGroupId",
                table: "OriginDestination",
                column: "CompanyGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_OriginDestination_CompanyGroup_CompanyGroupId",
                table: "OriginDestination",
                column: "CompanyGroupId",
                principalTable: "CompanyGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OriginDestination_CompanyGroup_CompanyGroupId",
                table: "OriginDestination");

            migrationBuilder.DropIndex(
                name: "IX_OriginDestination_CompanyGroupId",
                table: "OriginDestination");

            migrationBuilder.DropColumn(
                name: "CompanyGroupId",
                table: "OriginDestination");

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
    }
}
