using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_06192020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LockedEmployeeLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProcessedBy = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<int>(nullable: false),
                    Process = table.Column<string>(nullable: true),
                    ProcessedDate = table.Column<DateTime>(nullable: false),
                    LockedFrom = table.Column<DateTime>(nullable: true),
                    LockedTo = table.Column<DateTime>(nullable: true),
                    LockedRemarks = table.Column<string>(nullable: true),
                    LockedControlNo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LockedEmployeeLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LockedEmployeeLogs_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LockedEmployeeLogs_EmployeeId",
                table: "LockedEmployeeLogs",
                column: "EmployeeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LockedEmployeeLogs");
        }
    }
}
