using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class insertParcel231_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        -- 1. Create a temporary table with the new structure
        CREATE TABLE ParcelRequestIdParameter_Temp (
            id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
            Year INT NOT NULL
        );

        -- 2. Copy existing data
        INSERT INTO ParcelRequestIdParameter_Temp (Year)
        SELECT Year FROM ParcelRequestIdParameter;

        -- 3. Drop the old table
        DROP TABLE ParcelRequestIdParameter;

        -- 4. Rename the new table
        EXEC sp_rename 'ParcelRequestIdParameter_Temp', 'ParcelRequestIdParameter';
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(2025, 10, 13, 15, 13, 47, 961, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(2025, 10, 13, 15, 13, 47, 962, DateTimeKind.Local));
        }
    }
}
