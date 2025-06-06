using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class ScheduleRemoveDayOfWeekAddWeekStartDateWeekEndDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "Schedules");

            migrationBuilder.AddColumn<DateOnly>(
                name: "WeekEndDate",
                table: "Schedules",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "WeekStartDate",
                table: "Schedules",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeekEndDate",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "WeekStartDate",
                table: "Schedules");

            migrationBuilder.AddColumn<int>(
                name: "DayOfWeek",
                table: "Schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
