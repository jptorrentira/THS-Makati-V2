using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_01142020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DriverPassengers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServiceDateTimeStamp = table.Column<long>(nullable: false),
                    TransactionId = table.Column<string>(nullable: true),
                    ServiceDate = table.Column<DateTime>(nullable: true),
                    ShuttleId = table.Column<int>(nullable: false),
                    PassengerTypeId = table.Column<int>(nullable: false),
                    EmployeeNo = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<int>(nullable: false),
                    CompanyListId = table.Column<int>(nullable: false),
                    CompanyOther = table.Column<string>(nullable: true),
                    ChargingCompanyId = table.Column<int>(nullable: false),
                    ChargingDepartmentId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: false),
                    MiddleName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: false),
                    Position = table.Column<string>(nullable: false),
                    Purpose = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    Remarks = table.Column<string>(type: "VARCHAR(250)", nullable: false),
                    TripTypeId = table.Column<int>(nullable: false),
                    TripTimeFrom = table.Column<string>(nullable: true),
                    TripTimeTo = table.Column<string>(nullable: true),
                    Origin = table.Column<string>(nullable: true),
                    Destination = table.Column<string>(nullable: true),
                    EncodedBy = table.Column<string>(nullable: true),
                    LastModifiedBy = table.Column<string>(nullable: true),
                    EncodeDate = table.Column<DateTime>(nullable: true),
                    ModifyDate = table.Column<DateTime>(nullable: true),
                    SubmitDate = table.Column<DateTime>(nullable: true),
                    ContactNo = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    InitialApproverEmployeeNo = table.Column<string>(nullable: true),
                    ApprovedBy = table.Column<string>(nullable: true),
                    ApprovedDatetime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverPassengers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverPassengers_ChargingCompanys_ChargingCompanyId",
                        column: x => x.ChargingCompanyId,
                        principalTable: "ChargingCompanys",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DriverPassengers_ChargingDepartments_ChargingDepartmentId",
                        column: x => x.ChargingDepartmentId,
                        principalTable: "ChargingDepartments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DriverPassengers_CompanyLists_CompanyListId",
                        column: x => x.CompanyListId,
                        principalTable: "CompanyLists",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DriverPassengers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DriverPassengers_PassengerTypes_PassengerTypeId",
                        column: x => x.PassengerTypeId,
                        principalTable: "PassengerTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DriverPassengers_TripTypes_TripTypeId",
                        column: x => x.TripTypeId,
                        principalTable: "TripTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DriverPassengers_ChargingCompanyId",
                table: "DriverPassengers",
                column: "ChargingCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverPassengers_ChargingDepartmentId",
                table: "DriverPassengers",
                column: "ChargingDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverPassengers_CompanyListId",
                table: "DriverPassengers",
                column: "CompanyListId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverPassengers_EmployeeId",
                table: "DriverPassengers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverPassengers_PassengerTypeId",
                table: "DriverPassengers",
                column: "PassengerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverPassengers_TripTypeId",
                table: "DriverPassengers",
                column: "TripTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DriverPassengers");
        }
    }
}
