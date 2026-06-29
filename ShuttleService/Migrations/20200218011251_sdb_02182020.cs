using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_02182020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SMSBy",
                table: "Trip",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SMSDatetime",
                table: "Trip",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SMSStatus",
                table: "Trip",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SMSVersion",
                table: "Trip",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SMSBy",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "SMSDatetime",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "SMSStatus",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "SMSVersion",
                table: "Trip");
        }
    }
}
