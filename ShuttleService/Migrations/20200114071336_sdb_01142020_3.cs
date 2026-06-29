using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_01142020_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverPassengers_TripTypes_TripTypeId",
                table: "DriverPassengers");

            migrationBuilder.DropIndex(
                name: "IX_DriverPassengers_TripTypeId",
                table: "DriverPassengers");

            migrationBuilder.DropColumn(
                name: "TripTypeId",
                table: "DriverPassengers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TripTypeId",
                table: "DriverPassengers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DriverPassengers_TripTypeId",
                table: "DriverPassengers",
                column: "TripTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverPassengers_TripTypes_TripTypeId",
                table: "DriverPassengers",
                column: "TripTypeId",
                principalTable: "TripTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
