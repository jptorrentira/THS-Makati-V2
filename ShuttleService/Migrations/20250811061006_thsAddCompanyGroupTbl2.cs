using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class thsAddCompanyGroupTbl2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyGroupId",
                table: "VehicleLists",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyGroupdId",
                table: "VehicleLists",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "CompanyGroupId",
                table: "Employees",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyGroupdId",
                table: "Employees",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "CompanyGroupId",
                table: "Drivers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyGroupdId",
                table: "Drivers",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "CompanyGroupId",
                table: "CompanyLists",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyGroupdId",
                table: "CompanyLists",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLists_CompanyGroupId",
                table: "VehicleLists",
                column: "CompanyGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompanyGroupId",
                table: "Employees",
                column: "CompanyGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_CompanyGroupId",
                table: "Drivers",
                column: "CompanyGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyLists_CompanyGroupId",
                table: "CompanyLists",
                column: "CompanyGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyLists_CompanyGroup_CompanyGroupId",
                table: "CompanyLists",
                column: "CompanyGroupId",
                principalTable: "CompanyGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_CompanyGroup_CompanyGroupId",
                table: "Drivers",
                column: "CompanyGroupId",
                principalTable: "CompanyGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_CompanyGroup_CompanyGroupId",
                table: "Employees",
                column: "CompanyGroupId",
                principalTable: "CompanyGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleLists_CompanyGroup_CompanyGroupId",
                table: "VehicleLists",
                column: "CompanyGroupId",
                principalTable: "CompanyGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyLists_CompanyGroup_CompanyGroupId",
                table: "CompanyLists");

            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_CompanyGroup_CompanyGroupId",
                table: "Drivers");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_CompanyGroup_CompanyGroupId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleLists_CompanyGroup_CompanyGroupId",
                table: "VehicleLists");

            migrationBuilder.DropIndex(
                name: "IX_VehicleLists_CompanyGroupId",
                table: "VehicleLists");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CompanyGroupId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_CompanyGroupId",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_CompanyLists_CompanyGroupId",
                table: "CompanyLists");

            migrationBuilder.DropColumn(
                name: "CompanyGroupId",
                table: "VehicleLists");

            migrationBuilder.DropColumn(
                name: "CompanyGroupdId",
                table: "VehicleLists");

            migrationBuilder.DropColumn(
                name: "CompanyGroupId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CompanyGroupdId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CompanyGroupId",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "CompanyGroupdId",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "CompanyGroupId",
                table: "CompanyLists");

            migrationBuilder.DropColumn(
                name: "CompanyGroupdId",
                table: "CompanyLists");
        }
    }
}
