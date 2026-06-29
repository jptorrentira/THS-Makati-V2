using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_01152020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DriverPassengerHeaderId",
                table: "DriverPassengers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DriverPassengerHeader",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServiceDateTimeStamp = table.Column<long>(nullable: false),
                    TransactionId = table.Column<string>(nullable: true),
                    TempTransactionId = table.Column<string>(nullable: true),
                    ServiceDate = table.Column<DateTime>(nullable: true),
                    ShuttleId = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    Purpose = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    Remarks = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    TripTimeFrom = table.Column<string>(nullable: false),
                    TripTimeTo = table.Column<string>(nullable: false),
                    Origin = table.Column<string>(nullable: false),
                    Destination = table.Column<string>(nullable: false),
                    EncodedBy = table.Column<string>(nullable: true),
                    LastModifiedBy = table.Column<string>(nullable: true),
                    EncodeDate = table.Column<DateTime>(nullable: true),
                    ModifyDate = table.Column<DateTime>(nullable: true),
                    SubmitDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    InitialApproverEmployeeNo = table.Column<string>(nullable: true),
                    ApprovedBy = table.Column<string>(nullable: true),
                    ApprovedDatetime = table.Column<DateTime>(nullable: true),
                    ServiceTypeId = table.Column<int>(nullable: false),
                    Instructions = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverPassengerHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverPassengerHeader_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DriverPassengerHeader_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DriverPassengers_DriverPassengerHeaderId",
                table: "DriverPassengers",
                column: "DriverPassengerHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverPassengerHeader_EmployeeId",
                table: "DriverPassengerHeader",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverPassengerHeader_ServiceTypeId",
                table: "DriverPassengerHeader",
                column: "ServiceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverPassengers_DriverPassengerHeader_DriverPassengerHeaderId",
                table: "DriverPassengers",
                column: "DriverPassengerHeaderId",
                principalTable: "DriverPassengerHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverPassengers_DriverPassengerHeader_DriverPassengerHeaderId",
                table: "DriverPassengers");

            migrationBuilder.DropTable(
                name: "DriverPassengerHeader");

            migrationBuilder.DropIndex(
                name: "IX_DriverPassengers_DriverPassengerHeaderId",
                table: "DriverPassengers");

            migrationBuilder.DropColumn(
                name: "DriverPassengerHeaderId",
                table: "DriverPassengers");
        }
    }
}
