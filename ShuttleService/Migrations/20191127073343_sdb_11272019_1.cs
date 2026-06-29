using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_11272019_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_CompanyLists_CompanyListId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Nationalities_NationalityId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_ShuttlePassengers_ChargingCompanys_ChargingCompanyId",
                table: "ShuttlePassengers");

            migrationBuilder.DropForeignKey(
                name: "FK_ShuttlePassengers_ChargingDepartments_ChargingDepartmentId",
                table: "ShuttlePassengers");

            migrationBuilder.DropForeignKey(
                name: "FK_ShuttlePassengers_CompanyLists_CompanyListId",
                table: "ShuttlePassengers");

            migrationBuilder.DropForeignKey(
                name: "FK_ShuttlePassengers_Employees_EmployeeId",
                table: "ShuttlePassengers");

            migrationBuilder.DropForeignKey(
                name: "FK_ShuttlePassengers_PassengerTypes_PassengerTypeId",
                table: "ShuttlePassengers");

            migrationBuilder.DropForeignKey(
                name: "FK_ShuttlePassengers_TripTypes_TripTypeId",
                table: "ShuttlePassengers");

            migrationBuilder.DropForeignKey(
                name: "FK_Shuttles_Drivers_DriverId",
                table: "Shuttles");

            migrationBuilder.DropForeignKey(
                name: "FK_Shuttles_VehicleLists_VehicleListId",
                table: "Shuttles");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_CompanyLists_CompanyListId",
                table: "Employees",
                column: "CompanyListId",
                principalTable: "CompanyLists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Nationalities_NationalityId",
                table: "Employees",
                column: "NationalityId",
                principalTable: "Nationalities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShuttlePassengers_ChargingCompanys_ChargingCompanyId",
                table: "ShuttlePassengers",
                column: "ChargingCompanyId",
                principalTable: "ChargingCompanys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShuttlePassengers_ChargingDepartments_ChargingDepartmentId",
                table: "ShuttlePassengers",
                column: "ChargingDepartmentId",
                principalTable: "ChargingDepartments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShuttlePassengers_CompanyLists_CompanyListId",
                table: "ShuttlePassengers",
                column: "CompanyListId",
                principalTable: "CompanyLists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShuttlePassengers_Employees_EmployeeId",
                table: "ShuttlePassengers",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShuttlePassengers_PassengerTypes_PassengerTypeId",
                table: "ShuttlePassengers",
                column: "PassengerTypeId",
                principalTable: "PassengerTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShuttlePassengers_TripTypes_TripTypeId",
                table: "ShuttlePassengers",
                column: "TripTypeId",
                principalTable: "TripTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shuttles_Drivers_DriverId",
                table: "Shuttles",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shuttles_VehicleLists_VehicleListId",
                table: "Shuttles",
                column: "VehicleListId",
                principalTable: "VehicleLists",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_CompanyLists_CompanyListId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Nationalities_NationalityId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_ShuttlePassengers_ChargingCompanys_ChargingCompanyId",
                table: "ShuttlePassengers");

            migrationBuilder.DropForeignKey(
                name: "FK_ShuttlePassengers_ChargingDepartments_ChargingDepartmentId",
                table: "ShuttlePassengers");

            migrationBuilder.DropForeignKey(
                name: "FK_ShuttlePassengers_CompanyLists_CompanyListId",
                table: "ShuttlePassengers");

            migrationBuilder.DropForeignKey(
                name: "FK_ShuttlePassengers_Employees_EmployeeId",
                table: "ShuttlePassengers");

            migrationBuilder.DropForeignKey(
                name: "FK_ShuttlePassengers_PassengerTypes_PassengerTypeId",
                table: "ShuttlePassengers");

            migrationBuilder.DropForeignKey(
                name: "FK_ShuttlePassengers_TripTypes_TripTypeId",
                table: "ShuttlePassengers");

            migrationBuilder.DropForeignKey(
                name: "FK_Shuttles_Drivers_DriverId",
                table: "Shuttles");

            migrationBuilder.DropForeignKey(
                name: "FK_Shuttles_VehicleLists_VehicleListId",
                table: "Shuttles");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_CompanyLists_CompanyListId",
                table: "Employees",
                column: "CompanyListId",
                principalTable: "CompanyLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Nationalities_NationalityId",
                table: "Employees",
                column: "NationalityId",
                principalTable: "Nationalities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShuttlePassengers_ChargingCompanys_ChargingCompanyId",
                table: "ShuttlePassengers",
                column: "ChargingCompanyId",
                principalTable: "ChargingCompanys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShuttlePassengers_ChargingDepartments_ChargingDepartmentId",
                table: "ShuttlePassengers",
                column: "ChargingDepartmentId",
                principalTable: "ChargingDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShuttlePassengers_CompanyLists_CompanyListId",
                table: "ShuttlePassengers",
                column: "CompanyListId",
                principalTable: "CompanyLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShuttlePassengers_Employees_EmployeeId",
                table: "ShuttlePassengers",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShuttlePassengers_PassengerTypes_PassengerTypeId",
                table: "ShuttlePassengers",
                column: "PassengerTypeId",
                principalTable: "PassengerTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShuttlePassengers_TripTypes_TripTypeId",
                table: "ShuttlePassengers",
                column: "TripTypeId",
                principalTable: "TripTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shuttles_Drivers_DriverId",
                table: "Shuttles",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shuttles_VehicleLists_VehicleListId",
                table: "Shuttles",
                column: "VehicleListId",
                principalTable: "VehicleLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
