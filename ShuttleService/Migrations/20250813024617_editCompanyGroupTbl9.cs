using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class editCompanyGroupTbl9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyGroupId",
                table: "ChargingDepartments",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(2025, 8, 13, 10, 46, 17, 244, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(2025, 8, 13, 10, 46, 17, 244, DateTimeKind.Local));

            migrationBuilder.CreateIndex(
                name: "IX_ChargingDepartments_CompanyGroupId",
                table: "ChargingDepartments",
                column: "CompanyGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChargingDepartments_CompanyGroup_CompanyGroupId",
                table: "ChargingDepartments",
                column: "CompanyGroupId",
                principalTable: "CompanyGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChargingDepartments_CompanyGroup_CompanyGroupId",
                table: "ChargingDepartments");

            migrationBuilder.DropIndex(
                name: "IX_ChargingDepartments_CompanyGroupId",
                table: "ChargingDepartments");

            migrationBuilder.DropColumn(
                name: "CompanyGroupId",
                table: "ChargingDepartments");

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(2025, 8, 13, 10, 2, 12, 85, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(2025, 8, 13, 10, 2, 12, 86, DateTimeKind.Local));
        }
    }
}
