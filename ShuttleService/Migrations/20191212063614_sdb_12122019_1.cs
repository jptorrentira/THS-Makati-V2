using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_12122019_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DefaultApprover_Employees_ApproverEmployeeId",
                table: "DefaultApprover");

            migrationBuilder.DropForeignKey(
                name: "FK_DefaultApprover_Employees_EmployeeId",
                table: "DefaultApprover");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DefaultApprover",
                table: "DefaultApprover");

            migrationBuilder.RenameTable(
                name: "DefaultApprover",
                newName: "DefaultApprovers");

            migrationBuilder.RenameIndex(
                name: "IX_DefaultApprover_EmployeeId",
                table: "DefaultApprovers",
                newName: "IX_DefaultApprovers_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_DefaultApprover_ApproverEmployeeId",
                table: "DefaultApprovers",
                newName: "IX_DefaultApprovers_ApproverEmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DefaultApprovers",
                table: "DefaultApprovers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ReservationType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trip",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TripControlNo = table.Column<string>(nullable: true),
                    ServiceDateTimeStamp = table.Column<long>(nullable: false),
                    ServiceStartDate = table.Column<DateTime>(nullable: false),
                    ServiceEndDate = table.Column<DateTime>(nullable: false),
                    VehicleListId = table.Column<int>(nullable: false),
                    Capacity = table.Column<int>(nullable: false),
                    RemainingCapacity = table.Column<int>(nullable: false),
                    DriverId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ReservationTypeId = table.Column<int>(nullable: false),
                    Remarks = table.Column<string>(type: "VARCHAR(250)", nullable: true),
                    EncodedBy = table.Column<string>(nullable: false),
                    LastModifiedBy = table.Column<string>(nullable: true),
                    EncodeDate = table.Column<DateTime>(nullable: true),
                    ModifyDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trip", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trip_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trip_ReservationType_ReservationTypeId",
                        column: x => x.ReservationTypeId,
                        principalTable: "ReservationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trip_VehicleLists_VehicleListId",
                        column: x => x.VehicleListId,
                        principalTable: "VehicleLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trip_DriverId",
                table: "Trip",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Trip_ReservationTypeId",
                table: "Trip",
                column: "ReservationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Trip_VehicleListId",
                table: "Trip",
                column: "VehicleListId");

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultApprovers_Employees_ApproverEmployeeId",
                table: "DefaultApprovers",
                column: "ApproverEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultApprovers_Employees_EmployeeId",
                table: "DefaultApprovers",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DefaultApprovers_Employees_ApproverEmployeeId",
                table: "DefaultApprovers");

            migrationBuilder.DropForeignKey(
                name: "FK_DefaultApprovers_Employees_EmployeeId",
                table: "DefaultApprovers");

            migrationBuilder.DropTable(
                name: "Trip");

            migrationBuilder.DropTable(
                name: "ReservationType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DefaultApprovers",
                table: "DefaultApprovers");

            migrationBuilder.RenameTable(
                name: "DefaultApprovers",
                newName: "DefaultApprover");

            migrationBuilder.RenameIndex(
                name: "IX_DefaultApprovers_EmployeeId",
                table: "DefaultApprover",
                newName: "IX_DefaultApprover_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_DefaultApprovers_ApproverEmployeeId",
                table: "DefaultApprover",
                newName: "IX_DefaultApprover_ApproverEmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DefaultApprover",
                table: "DefaultApprover",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultApprover_Employees_ApproverEmployeeId",
                table: "DefaultApprover",
                column: "ApproverEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DefaultApprover_Employees_EmployeeId",
                table: "DefaultApprover",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
