using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_02272020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SMSRevision",
                table: "ShuttlePassengers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SMSRevision",
                table: "DriverPassengers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SMSRevision",
                table: "ShuttlePassengers");

            migrationBuilder.DropColumn(
                name: "SMSRevision",
                table: "DriverPassengers");
        }
    }
}
