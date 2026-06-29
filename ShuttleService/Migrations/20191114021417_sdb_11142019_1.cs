using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_11142019_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "ShuttlePassengers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShuttlePassengers_EmployeeId",
                table: "ShuttlePassengers",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShuttlePassengers_Employees_EmployeeId",
                table: "ShuttlePassengers",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShuttlePassengers_Employees_EmployeeId",
                table: "ShuttlePassengers");

            migrationBuilder.DropIndex(
                name: "IX_ShuttlePassengers_EmployeeId",
                table: "ShuttlePassengers");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "ShuttlePassengers");
        }
    }
}
