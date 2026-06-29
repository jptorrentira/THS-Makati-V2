using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class insertParcel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverLocation_Drivers_DriverId",
                table: "DriverLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverLocation_Location_LocationId",
                table: "DriverLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLocation_Employees_EmployeeId",
                table: "EmployeeLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLocation_Location_LocationId",
                table: "EmployeeLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleLocation_Location_LocationId",
                table: "VehicleLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleLocation_VehicleLists_VehicleId",
                table: "VehicleLocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleLocation",
                table: "VehicleLocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeLocation",
                table: "EmployeeLocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DriverLocation",
                table: "DriverLocation");

            migrationBuilder.RenameTable(
                name: "VehicleLocation",
                newName: "VehicleLocations");

            migrationBuilder.RenameTable(
                name: "EmployeeLocation",
                newName: "EmployeeLocations");

            migrationBuilder.RenameTable(
                name: "DriverLocation",
                newName: "DriverLocations");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleLocation_LocationId",
                table: "VehicleLocations",
                newName: "IX_VehicleLocations_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeLocation_LocationId",
                table: "EmployeeLocations",
                newName: "IX_EmployeeLocations_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_DriverLocation_LocationId",
                table: "DriverLocations",
                newName: "IX_DriverLocations_LocationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleLocations",
                table: "VehicleLocations",
                columns: new[] { "VehicleId", "LocationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeLocations",
                table: "EmployeeLocations",
                columns: new[] { "EmployeeId", "LocationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DriverLocations",
                table: "DriverLocations",
                columns: new[] { "DriverId", "LocationId" });

            migrationBuilder.CreateTable(
                name: "ParcelCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Category = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParcelCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParcelDeliveries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChargingCompanyId = table.Column<int>(nullable: false),
                    ChargingDepartmentId = table.Column<int>(nullable: false),
                    ParcelCategoryId = table.Column<string>(nullable: false),
                    DestinationId = table.Column<int>(nullable: false),
                    Destination = table.Column<string>(nullable: false),
                    Recipient = table.Column<string>(nullable: false),
                    RecipientEmail = table.Column<string>(nullable: false),
                    RecipientDepartment = table.Column<string>(nullable: false),
                    DateTimeToBeReceived = table.Column<DateTime>(nullable: false),
                    ParcelDescription = table.Column<string>(nullable: false),
                    Instruction = table.Column<string>(nullable: false),
                    ActualDispatchDate = table.Column<DateTime>(nullable: false),
                    ActualReceivedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedDated = table.Column<DateTime>(nullable: false),
                    RejectedBy = table.Column<string>(nullable: true),
                    RejectedDate = table.Column<DateTime>(nullable: false),
                    RejectionReason = table.Column<string>(nullable: true),
                    FiledBy = table.Column<string>(nullable: true),
                    FiledDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParcelDeliveries", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(2025, 10, 13, 10, 50, 3, 383, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(2025, 10, 13, 10, 50, 3, 385, DateTimeKind.Local));

            migrationBuilder.AddForeignKey(
                name: "FK_DriverLocations_Drivers_DriverId",
                table: "DriverLocations",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverLocations_Location_LocationId",
                table: "DriverLocations",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLocations_Employees_EmployeeId",
                table: "EmployeeLocations",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLocations_Location_LocationId",
                table: "EmployeeLocations",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleLocations_Location_LocationId",
                table: "VehicleLocations",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleLocations_VehicleLists_VehicleId",
                table: "VehicleLocations",
                column: "VehicleId",
                principalTable: "VehicleLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverLocations_Drivers_DriverId",
                table: "DriverLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverLocations_Location_LocationId",
                table: "DriverLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLocations_Employees_EmployeeId",
                table: "EmployeeLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLocations_Location_LocationId",
                table: "EmployeeLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleLocations_Location_LocationId",
                table: "VehicleLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleLocations_VehicleLists_VehicleId",
                table: "VehicleLocations");

            migrationBuilder.DropTable(
                name: "ParcelCategories");

            migrationBuilder.DropTable(
                name: "ParcelDeliveries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleLocations",
                table: "VehicleLocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeLocations",
                table: "EmployeeLocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DriverLocations",
                table: "DriverLocations");

            migrationBuilder.RenameTable(
                name: "VehicleLocations",
                newName: "VehicleLocation");

            migrationBuilder.RenameTable(
                name: "EmployeeLocations",
                newName: "EmployeeLocation");

            migrationBuilder.RenameTable(
                name: "DriverLocations",
                newName: "DriverLocation");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleLocations_LocationId",
                table: "VehicleLocation",
                newName: "IX_VehicleLocation_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeLocations_LocationId",
                table: "EmployeeLocation",
                newName: "IX_EmployeeLocation_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_DriverLocations_LocationId",
                table: "DriverLocation",
                newName: "IX_DriverLocation_LocationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleLocation",
                table: "VehicleLocation",
                columns: new[] { "VehicleId", "LocationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeLocation",
                table: "EmployeeLocation",
                columns: new[] { "EmployeeId", "LocationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DriverLocation",
                table: "DriverLocation",
                columns: new[] { "DriverId", "LocationId" });

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 1,
                column: "InsertDate",
                value: new DateTime(2025, 10, 2, 17, 0, 4, 471, DateTimeKind.Local));

            migrationBuilder.UpdateData(
                table: "CompanyGroup",
                keyColumn: "Id",
                keyValue: 2,
                column: "InsertDate",
                value: new DateTime(2025, 10, 2, 17, 0, 4, 472, DateTimeKind.Local));

            migrationBuilder.AddForeignKey(
                name: "FK_DriverLocation_Drivers_DriverId",
                table: "DriverLocation",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverLocation_Location_LocationId",
                table: "DriverLocation",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLocation_Employees_EmployeeId",
                table: "EmployeeLocation",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLocation_Location_LocationId",
                table: "EmployeeLocation",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleLocation_Location_LocationId",
                table: "VehicleLocation",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleLocation_VehicleLists_VehicleId",
                table: "VehicleLocation",
                column: "VehicleId",
                principalTable: "VehicleLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
