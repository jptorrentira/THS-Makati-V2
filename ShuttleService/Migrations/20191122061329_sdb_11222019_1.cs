using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_11222019_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalApproverEmployeeNo",
                table: "ShuttlePassengers");

            migrationBuilder.AddColumn<int>(
                name: "is_recomputed",
                table: "Employees",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_recomputed",
                table: "Employees");

            migrationBuilder.AddColumn<string>(
                name: "OriginalApproverEmployeeNo",
                table: "ShuttlePassengers",
                nullable: true);
        }
    }
}
