using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_12122019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DefaultApprovers_Employees_ApproverEmployeeId",
                table: "DefaultApprovers");

            migrationBuilder.DropForeignKey(
                name: "FK_DefaultApprovers_Employees_EmployeeId",
                table: "DefaultApprovers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DefaultApprovers",
                table: "DefaultApprovers");

            migrationBuilder.RenameTable(
                name: "DefaultApprovers",
                newName: "DefaultApprover");

            migrationBuilder.RenameIndex(
                name: "IX_DefaultApprovers_EmployeeId",
                table: "DefaultApprover",
                newName: "IX_DefaultApprover_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_DefaultApprovers_ApproverEmployeeId",
                table: "DefaultApprover",
                newName: "IX_DefaultApprover_ApproverEmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DefaultApprover",
                table: "DefaultApprover",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultApprover_Employees_ApproverEmployeeId",
                table: "DefaultApprover",
                column: "ApproverEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultApprover_Employees_EmployeeId",
                table: "DefaultApprover",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DefaultApprover_Employees_ApproverEmployeeId",
                table: "DefaultApprover");

            migrationBuilder.DropForeignKey(
                name: "FK_DefaultApprover_Employees_EmployeeId",
                table: "DefaultApprover");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DefaultApprover",
                table: "DefaultApprover");

            migrationBuilder.RenameTable(
                name: "DefaultApprover",
                newName: "DefaultApprovers");

            migrationBuilder.RenameIndex(
                name: "IX_DefaultApprover_EmployeeId",
                table: "DefaultApprovers",
                newName: "IX_DefaultApprovers_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_DefaultApprover_ApproverEmployeeId",
                table: "DefaultApprovers",
                newName: "IX_DefaultApprovers_ApproverEmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DefaultApprovers",
                table: "DefaultApprovers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultApprovers_Employees_ApproverEmployeeId",
                table: "DefaultApprovers",
                column: "ApproverEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultApprovers_Employees_EmployeeId",
                table: "DefaultApprovers",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
