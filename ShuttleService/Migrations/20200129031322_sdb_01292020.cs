using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_01292020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SmsId",
                table: "ShuttlePassengers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmsId",
                table: "DriverPassengers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SmsId",
                table: "ShuttlePassengers");

            migrationBuilder.DropColumn(
                name: "SmsId",
                table: "DriverPassengers");
        }
    }
}
