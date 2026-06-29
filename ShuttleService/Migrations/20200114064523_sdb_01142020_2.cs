using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_01142020_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceTypeId",
                table: "DriverPassengers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ServiceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DriverPassengers_ServiceTypeId",
                table: "DriverPassengers",
                column: "ServiceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverPassengers_ServiceTypes_ServiceTypeId",
                table: "DriverPassengers",
                column: "ServiceTypeId",
                principalTable: "ServiceTypes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverPassengers_ServiceTypes_ServiceTypeId",
                table: "DriverPassengers");

            migrationBuilder.DropTable(
                name: "ServiceTypes");

            migrationBuilder.DropIndex(
                name: "IX_DriverPassengers_ServiceTypeId",
                table: "DriverPassengers");

            migrationBuilder.DropColumn(
                name: "ServiceTypeId",
                table: "DriverPassengers");
        }
    }
}
