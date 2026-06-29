using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_05272021_105pm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DestinationTag",
                table: "DriverPassengers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginTag",
                table: "DriverPassengers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DestinationTag",
                table: "DriverPassengerHeaders",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginTag",
                table: "DriverPassengerHeaders",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationTag",
                table: "DriverPassengers");

            migrationBuilder.DropColumn(
                name: "OriginTag",
                table: "DriverPassengers");

            migrationBuilder.DropColumn(
                name: "DestinationTag",
                table: "DriverPassengerHeaders");

            migrationBuilder.DropColumn(
                name: "OriginTag",
                table: "DriverPassengerHeaders");
        }
    }
}
