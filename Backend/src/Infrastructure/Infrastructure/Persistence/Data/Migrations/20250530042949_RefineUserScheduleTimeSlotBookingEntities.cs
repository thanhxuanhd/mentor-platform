using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefineUserScheduleTimeSlotBookingEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_timeSlots_Schedules_ScheduleId",
                table: "timeSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_timeSlots_Users_MentorId",
                table: "timeSlots");

            migrationBuilder.DropIndex(
                name: "IX_timeSlots_ScheduleId",
                table: "timeSlots");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "timeSlots");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "bookings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_LearnerId",
                table: "bookings",
                column: "LearnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_bookings_Users_LearnerId",
                table: "bookings",
                column: "LearnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_timeSlots_Users_MentorId",
                table: "timeSlots",
                column: "MentorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookings_Users_LearnerId",
                table: "bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_timeSlots_Users_MentorId",
                table: "timeSlots");

            migrationBuilder.DropIndex(
                name: "IX_bookings_LearnerId",
                table: "bookings");

            migrationBuilder.AddColumn<Guid>(
                name: "ScheduleId",
                table: "timeSlots",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "bookings",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_timeSlots_ScheduleId",
                table: "timeSlots",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_timeSlots_Schedules_ScheduleId",
                table: "timeSlots",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_timeSlots_Users_MentorId",
                table: "timeSlots",
                column: "MentorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
