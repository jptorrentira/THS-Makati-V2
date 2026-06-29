using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class editCompanyGroupTbl8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_CompanyLists_CompanyListId",
                table: "Drivers");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleLists_CompanyLists_CompanyListId",
                table: "VehicleLists");

            migrationBuilder.DropIndex(
                name: "IX_VehicleLists_CompanyListId",
                table: "VehicleLists");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_CompanyListId",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "CompanyListId",
                table: "VehicleLists");

            migrationBuilder.DropColumn(
                name: "CompanyListId",
                table: "Drivers");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyGroupId",
                table: "DepartmentLists",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CompanyGroupId",
                table: "CompanyLists",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "CompanyGroupId",
                table: "ChargingCompanys",
                nullable: false,
                defaultValue: 1);

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

            migrationBuilder.CreateIndex(
                name: "IX_ChargingCompanys_CompanyGroupId",
                table: "ChargingCompanys",
                column: "CompanyGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChargingCompanys_CompanyGroup_CompanyGroupId",
                table: "ChargingCompanys",
                column: "CompanyGroupId",
                principalTable: "CompanyGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChargingCompanys_CompanyGroup_CompanyGroupId",
                table: "ChargingCompanys");

            migrationBuilder.DropIndex(
                name: "IX_ChargingCompanys_CompanyGroupId",
                table: "ChargingCompanys");

            migrationBuilder.DropColumn(
                name: "CompanyGroupId",
                table: "ChargingCompanys");

            migrationBuilder.AddColumn<int>(
                name: "CompanyListId",
                table: "VehicleLists",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "CompanyListId",
                table: "Drivers",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyGroupId",
                table: "DepartmentLists",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyGroupId",
                table: "CompanyLists",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValue: 1);

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLists_CompanyListId",
                table: "VehicleLists",
                column: "CompanyListId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_CompanyListId",
                table: "Drivers",
                column: "CompanyListId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_CompanyLists_CompanyListId",
                table: "Drivers",
                column: "CompanyListId",
                principalTable: "CompanyLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleLists_CompanyLists_CompanyListId",
                table: "VehicleLists",
                column: "CompanyListId",
                principalTable: "CompanyLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
