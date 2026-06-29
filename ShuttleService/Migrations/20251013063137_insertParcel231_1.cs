using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class insertParcel231_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedDated",
                table: "ParcelDeliveries",
                newName: "UpdatedDate");

            migrationBuilder.AlterColumn<int>(
                name: "ParcelCategoryId",
                table: "ParcelDeliveries",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "RequestId",
                table: "ParcelDeliveries",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ParcelRequestIdParameter",
                columns: table => new
                {
                    Year = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LastNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParcelRequestIdParameter", x => x.Year);
                });

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(2025, 10, 13, 14, 31, 37, 268, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(2025, 10, 13, 14, 31, 37, 268, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParcelRequestIdParameter");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "ParcelDeliveries");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "ParcelDeliveries",
                newName: "UpdatedDated");

            migrationBuilder.AlterColumn<string>(
                name: "ParcelCategoryId",
                table: "ParcelDeliveries",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(2025, 10, 13, 10, 55, 38, 97, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(2025, 10, 13, 10, 55, 38, 98, DateTimeKind.Local));
        }
    }
}
