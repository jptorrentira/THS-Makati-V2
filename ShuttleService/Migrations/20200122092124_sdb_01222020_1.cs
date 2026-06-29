using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_01222020_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "Trip",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledBy",
                table: "Trip",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledDatetime",
                table: "Trip",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "CancelledBy",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "CancelledDatetime",
                table: "Trip");
        }
    }
}
