using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixUserScheduleTimeSlotSessionEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_timeSlots_Users_MentorId",
                table: "timeSlots");

            migrationBuilder.DropTable(
                name: "bookings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_timeSlots",
                table: "timeSlots");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "Schedules");

            migrationBuilder.RenameTable(
                name: "timeSlots",
                newName: "MentorAvailableTimeSlots");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Schedules",
                newName: "StartHour");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "Schedules",
                newName: "EndHour");

            migrationBuilder.RenameColumn(
                name: "MentorId",
                table: "MentorAvailableTimeSlots",
                newName: "ScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_timeSlots_MentorId",
                table: "MentorAvailableTimeSlots",
                newName: "IX_MentorAvailableTimeSlots_ScheduleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MentorAvailableTimeSlots",
                table: "MentorAvailableTimeSlots",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeSlotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LearnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_MentorAvailableTimeSlots_TimeSlotId",
                        column: x => x.TimeSlotId,
                        principalTable: "MentorAvailableTimeSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_LearnerId",
                        column: x => x.LearnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_LearnerId",
                table: "Sessions",
                column: "LearnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_TimeSlotId",
                table: "Sessions",
                column: "TimeSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_MentorAvailableTimeSlots_Schedules_ScheduleId",
                table: "MentorAvailableTimeSlots",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MentorAvailableTimeSlots_Schedules_ScheduleId",
                table: "MentorAvailableTimeSlots");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MentorAvailableTimeSlots",
                table: "MentorAvailableTimeSlots");

            migrationBuilder.RenameTable(
                name: "MentorAvailableTimeSlots",
                newName: "timeSlots");

            migrationBuilder.RenameColumn(
                name: "StartHour",
                table: "Schedules",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "EndHour",
                table: "Schedules",
                newName: "EndTime");

            migrationBuilder.RenameColumn(
                name: "ScheduleId",
                table: "timeSlots",
                newName: "MentorId");

            migrationBuilder.RenameIndex(
                name: "IX_MentorAvailableTimeSlots_ScheduleId",
                table: "timeSlots",
                newName: "IX_timeSlots_MentorId");

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "Schedules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_timeSlots",
                table: "timeSlots",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LearnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeSlotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bookings_Users_LearnerId",
                        column: x => x.LearnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_bookings_timeSlots_TimeSlotId",
                        column: x => x.TimeSlotId,
                        principalTable: "timeSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bookings_LearnerId",
                table: "bookings",
                column: "LearnerId");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_TimeSlotId",
                table: "bookings",
                column: "TimeSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_timeSlots_Users_MentorId",
                table: "timeSlots",
                column: "MentorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
