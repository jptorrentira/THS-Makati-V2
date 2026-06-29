using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class sdb_01242020_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailBy",
                table: "Trip",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailDatetime",
                table: "Trip",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmailStatus",
                table: "Trip",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailBy",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "EmailDatetime",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "EmailStatus",
                table: "Trip");
        }
    }
}
