using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_01092020_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Drivers",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "CivilStatus",
                table: "Drivers",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CivilStatus",
                table: "Drivers");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Drivers",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
