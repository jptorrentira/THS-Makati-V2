using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_11282019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_CompanyLists_CompanyListId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
             name: "FK_Employees_Nationalities_NationalityId",
             table: "Employees");

            migrationBuilder.AddColumn<int>(
                name: "ChargingCompanyId",
                table: "Employees",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChargingDepartmentId",
                table: "Employees",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentListId",
                table: "Employees",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_CompanyLists_CompanyListId",
                table: "Employees",
                column: "CompanyListId",
                principalTable: "CompanyLists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Nationalities_NationalityId",
                table: "Employees",
                column: "NationalityId",
                principalTable: "Nationalities",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_CompanyLists_CompanyListId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Nationalities_NationalityId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ChargingCompanyId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ChargingDepartmentId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DepartmentListId",
                table: "Employees");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_CompanyLists_CompanyListId",
                table: "Employees",
                column: "CompanyListId",
                principalTable: "CompanyLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
