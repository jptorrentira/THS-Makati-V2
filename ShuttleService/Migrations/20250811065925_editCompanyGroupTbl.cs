using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class editCompanyGroupTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "CompanyGroupdId",
                table: "VehicleLists");

            migrationBuilder.DropColumn(
                name: "CompanyGroupdId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CompanyGroupdId",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "CompanyGroupdId",
                table: "CompanyLists");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyGroupId",
                table: "VehicleLists",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyGroupId",
                table: "Employees",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyGroupId",
                table: "Drivers",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyGroupId",
                table: "CompanyLists",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyLists_CompanyGroup_CompanyGroupId",
                table: "CompanyLists",
                column: "CompanyGroupId",
                principalTable: "CompanyGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_CompanyGroup_CompanyGroupId",
                table: "Drivers",
                column: "CompanyGroupId",
                principalTable: "CompanyGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_CompanyGroup_CompanyGroupId",
                table: "Employees",
                column: "CompanyGroupId",
                principalTable: "CompanyGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleLists_CompanyGroup_CompanyGroupId",
                table: "VehicleLists",
                column: "CompanyGroupId",
                principalTable: "CompanyGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
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

            migrationBuilder.AlterColumn<int>(
                name: "CompanyGroupId",
                table: "VehicleLists",
                nullable: true,
                oldClrType: typeof(int),
                oldDefaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "CompanyGroupdId",
                table: "VehicleLists",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyGroupId",
                table: "Employees",
                nullable: true,
                oldClrType: typeof(int),
                oldDefaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "CompanyGroupdId",
                table: "Employees",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyGroupId",
                table: "Drivers",
                nullable: true,
                oldClrType: typeof(int),
                oldDefaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "CompanyGroupdId",
                table: "Drivers",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyGroupId",
                table: "CompanyLists",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "CompanyGroupdId",
                table: "CompanyLists",
                nullable: false,
                defaultValue: 0);

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
    }
}
