using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class thsAddCompanyToDriver : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyListId",
                table: "Drivers",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_CompanyListId",
                table: "Drivers",
                column: "CompanyListId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_CompanyLists_CompanyListId",
                table: "Drivers",
                column: "CompanyListId",
                principalTable: "CompanyLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_CompanyLists_CompanyListId",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_CompanyListId",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "CompanyListId",
                table: "Drivers");
        }
    }
}
