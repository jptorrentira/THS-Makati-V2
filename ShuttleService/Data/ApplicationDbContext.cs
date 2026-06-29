using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShuttleService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShuttleService.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        public DbSet<ChargingCompany> ChargingCompanys { get; set; }
        public DbSet<ChargingDepartment> ChargingDepartments { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<DayNum> DayNums { get; set; }
        public DbSet<Shuttle> Shuttles { get; set; }
        public DbSet<ShuttlePassenger> ShuttlePassengers { get; set; }
        public DbSet<DriverPassenger> DriverPassengers { get; set; }
        public DbSet<VehicleList> VehicleLists { get; set; }
        public DbSet<Location>  Location { get; set; }
        public DbSet<OriginDestination> OriginDestination { get; set; }
        public DbSet<DepartmentList>  DepartmentLists{ get; set; }
        public DbSet<CompanyList> CompanyLists { get; set; }
        public DbSet<CompanyGroup> CompanyGroup { get; set; }
        public DbSet<PassengerType> PassengerTypes{ get; set; }
        public DbSet<TripType>  TripTypes{ get; set; }
        public DbSet<Employee>  Employees{ get; set; }
        public DbSet<Nationality> Nationalities { get; set; }
        public DbSet<DefaultApprover> DefaultApprovers { get; set; }
        public DbSet<Trip> Trip { get; set; }
        public DbSet<ReservationType> ReservationType { get; set; }
        public DbSet<VehicleMonitor> VehicleMonitor { get; set; }

        public DbSet<ServiceType> ServiceTypes{ get; set; }
        public DbSet<Log> Logs  { get; set; }

        public DbSet<MaintenanceLog> MaintenanceLogs { get; set; }
        public DbSet<DriverPassengerHeader> DriverPassengerHeaders { get; set; }
        public DbSet<EmailRecipient> EmailRecipients { get; set; }
        public DbSet<LockedEmployeeLog> LockedEmployeeLogs { get; set; }



        public DbSet<ShuttlePassengerStatus> ShuttlePassengerStatuses { get; set; }
        public DbSet<SmsTransactionCode> SmsTransactionCodes { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<SMSLog> SMSLogs { get; set; }

        public DbSet<SurveyTransaction> SurveyTransactions { get; set; }
        public DbSet<SurveyQuestion> SurveyQuestions{ get; set; }
        public DbSet<SurveyAnswer> SurveyAnswers { get; set; }

        public DbSet<EmployeeLocation> EmployeeLocations { get; set; }
        public DbSet<DriverLocation> DriverLocations { get; set; }
        public DbSet<VehicleLocation> VehicleLocations { get; set; }

        public DbSet<ParcelCategory> ParcelCategories { get; set; }
        public DbSet<ParcelDelivery> ParcelDeliveries { get; set; }

        public DbSet<ParcelRequestIdParameter> ParcelRequestIdParameter { get; set; }

        public DbSet<ShuttleService.Models.ApplicationRole> ApplicationRole { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .Property(d => d.CompanyListId)
                .HasDefaultValue(1);
            modelBuilder.Entity<Employee>()
                .Property(d => d.CompanyGroupId)
                .HasDefaultValue(1);
            modelBuilder.Entity<Driver>()
                .Property(d => d.CompanyGroupId)
                .HasDefaultValue(1);
            modelBuilder.Entity<ChargingCompany>()
                .Property(d => d.CompanyGroupId)
                .HasDefaultValue(1);
            modelBuilder.Entity<ChargingDepartment>()
                .Property(d => d.CompanyGroupId)
                .HasDefaultValue(1);
            modelBuilder.Entity<CompanyList>()
               .Property(d => d.CompanyGroupId)
               .HasDefaultValue(1);
            modelBuilder.Entity<VehicleList>()
                .Property(d => d.CompanyGroupId)
                .HasDefaultValue(1);
            modelBuilder.Entity<DepartmentList>()
                .Property(d => d.CompanyGroupId)
                .HasDefaultValue(1);
            modelBuilder.Entity<CompanyGroup>().HasData(
                new CompanyGroup { Id = 1, CompanyGroupName = "SMPC" },
                new CompanyGroup { Id = 2, CompanyGroupName = "UPDI" }
            );

            // Many to Many Employee to Location
            modelBuilder.Entity<EmployeeLocation>()
                .HasKey(el => new { el.EmployeeId, el.LocationId });

            modelBuilder.Entity<EmployeeLocation>()
                .HasOne(el => el.Employee)
                .WithMany(e => e.EmployeeLocations)
                .HasForeignKey(el => el.EmployeeId);

            modelBuilder.Entity<EmployeeLocation>()
                .HasOne(el => el.Location)
                .WithMany(l => l.EmployeeLocations)
                .HasForeignKey(el => el.LocationId);

            // Many to Many Driver to Location
            modelBuilder.Entity<DriverLocation>()
                .HasKey(el => new { el.DriverId, el.LocationId });

            modelBuilder.Entity<DriverLocation>()
                .HasOne(el => el.Driver)
                .WithMany(e => e.DriverLocations)
                .HasForeignKey(el => el.DriverId);

            modelBuilder.Entity<DriverLocation>()
                .HasOne(el => el.Location)
                .WithMany(l => l.DriverLocations)
                .HasForeignKey(el => el.LocationId);

            // Many to Many Driver to Location
            modelBuilder.Entity<VehicleLocation>()
                .HasKey(el => new { el.VehicleId, el.LocationId });

            modelBuilder.Entity<VehicleLocation>()
                .HasOne(el => el.Vehicle)
                .WithMany(e => e.VehicleLocations)
                .HasForeignKey(el => el.VehicleId);

            modelBuilder.Entity<VehicleLocation>()
                .HasOne(el => el.Location)
                .WithMany(l => l.VehicleLocations)
                .HasForeignKey(el => el.LocationId);

        }

    }
}
