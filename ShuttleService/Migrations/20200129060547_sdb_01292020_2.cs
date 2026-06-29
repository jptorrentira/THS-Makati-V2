using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_01292020_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalApproverEmployeeNo",
                table: "SmsTransactionCodes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalApproverEmployeeNo",
                table: "ShuttlePassengers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalApproverEmployeeNo",
                table: "DriverPassengers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalApproverEmployeeNo",
                table: "SmsTransactionCodes");

            migrationBuilder.DropColumn(
                name: "OriginalApproverEmployeeNo",
                table: "ShuttlePassengers");

            migrationBuilder.DropColumn(
                name: "OriginalApproverEmployeeNo",
                table: "DriverPassengers");
        }
    }
}
