using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_01152020_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverPassengerHeader_Employees_EmployeeId",
                table: "DriverPassengerHeader");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverPassengerHeader_ServiceTypes_ServiceTypeId",
                table: "DriverPassengerHeader");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverPassengers_DriverPassengerHeader_DriverPassengerHeaderId",
                table: "DriverPassengers");

            migrationBuilder.DropIndex(
                name: "IX_DriverPassengers_DriverPassengerHeaderId",
                table: "DriverPassengers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DriverPassengerHeader",
                table: "DriverPassengerHeader");

            migrationBuilder.RenameTable(
                name: "DriverPassengerHeader",
                newName: "DriverPassengerHeaders");

            migrationBuilder.RenameIndex(
                name: "IX_DriverPassengerHeader_ServiceTypeId",
                table: "DriverPassengerHeaders",
                newName: "IX_DriverPassengerHeaders_ServiceTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_DriverPassengerHeader_EmployeeId",
                table: "DriverPassengerHeaders",
                newName: "IX_DriverPassengerHeaders_EmployeeId");

            migrationBuilder.AlterColumn<string>(
                name: "DriverPassengerHeaderId",
                table: "DriverPassengers",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DriverPassengerHeaderId1",
                table: "DriverPassengers",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DriverPassengerHeaders",
                table: "DriverPassengerHeaders",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DriverPassengers_DriverPassengerHeaderId1",
                table: "DriverPassengers",
                column: "DriverPassengerHeaderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverPassengerHeaders_Employees_EmployeeId",
                table: "DriverPassengerHeaders",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverPassengerHeaders_ServiceTypes_ServiceTypeId",
                table: "DriverPassengerHeaders",
                column: "ServiceTypeId",
                principalTable: "ServiceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverPassengers_DriverPassengerHeaders_DriverPassengerHeaderId1",
                table: "DriverPassengers",
                column: "DriverPassengerHeaderId1",
                principalTable: "DriverPassengerHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverPassengerHeaders_Employees_EmployeeId",
                table: "DriverPassengerHeaders");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverPassengerHeaders_ServiceTypes_ServiceTypeId",
                table: "DriverPassengerHeaders");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverPassengers_DriverPassengerHeaders_DriverPassengerHeaderId1",
                table: "DriverPassengers");

            migrationBuilder.DropIndex(
                name: "IX_DriverPassengers_DriverPassengerHeaderId1",
                table: "DriverPassengers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DriverPassengerHeaders",
                table: "DriverPassengerHeaders");

            migrationBuilder.DropColumn(
                name: "DriverPassengerHeaderId1",
                table: "DriverPassengers");

            migrationBuilder.RenameTable(
                name: "DriverPassengerHeaders",
                newName: "DriverPassengerHeader");

            migrationBuilder.RenameIndex(
                name: "IX_DriverPassengerHeaders_ServiceTypeId",
                table: "DriverPassengerHeader",
                newName: "IX_DriverPassengerHeader_ServiceTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_DriverPassengerHeaders_EmployeeId",
                table: "DriverPassengerHeader",
                newName: "IX_DriverPassengerHeader_EmployeeId");

            migrationBuilder.AlterColumn<int>(
                name: "DriverPassengerHeaderId",
                table: "DriverPassengers",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DriverPassengerHeader",
                table: "DriverPassengerHeader",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DriverPassengers_DriverPassengerHeaderId",
                table: "DriverPassengers",
                column: "DriverPassengerHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverPassengerHeader_Employees_EmployeeId",
                table: "DriverPassengerHeader",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverPassengerHeader_ServiceTypes_ServiceTypeId",
                table: "DriverPassengerHeader",
                column: "ServiceTypeId",
                principalTable: "ServiceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverPassengers_DriverPassengerHeader_DriverPassengerHeaderId",
                table: "DriverPassengers",
                column: "DriverPassengerHeaderId",
                principalTable: "DriverPassengerHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
