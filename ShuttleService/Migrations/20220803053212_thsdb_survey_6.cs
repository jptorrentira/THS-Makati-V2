using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShuttleService.Migrations
{
    public partial class thsdb_survey_6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AnsweredDateTime",
                table: "SurveyTransactions",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GeneratedDateTime",
                table: "SurveyTransactions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnsweredDateTime",
                table: "SurveyTransactions");

            migrationBuilder.DropColumn(
                name: "GeneratedDateTime",
                table: "SurveyTransactions");
        }
    }
}
