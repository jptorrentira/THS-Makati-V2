using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class thsAddCompanyToVehicle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyListId",
                table: "VehicleLists",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLists_CompanyListId",
                table: "VehicleLists",
                column: "CompanyListId");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleLists_CompanyLists_CompanyListId",
                table: "VehicleLists",
                column: "CompanyListId",
                principalTable: "CompanyLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleLists_CompanyLists_CompanyListId",
                table: "VehicleLists");

            migrationBuilder.DropIndex(
                name: "IX_VehicleLists_CompanyListId",
                table: "VehicleLists");

            migrationBuilder.DropColumn(
                name: "CompanyListId",
                table: "VehicleLists");
        }
    }
}
