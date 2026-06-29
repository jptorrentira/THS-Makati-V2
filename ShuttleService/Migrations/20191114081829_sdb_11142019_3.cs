using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_11142019_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShuttlePassengers_Shuttles_ShuttleId",
                table: "ShuttlePassengers");

            migrationBuilder.DropIndex(
                name: "IX_ShuttlePassengers_ShuttleId",
                table: "ShuttlePassengers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ShuttlePassengers_ShuttleId",
                table: "ShuttlePassengers",
                column: "ShuttleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShuttlePassengers_Shuttles_ShuttleId",
                table: "ShuttlePassengers",
                column: "ShuttleId",
                principalTable: "Shuttles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
