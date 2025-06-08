using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixTimeSlotConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_MentorAvailableTimeSlots_TimeSlotId",
                table: "Sessions");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_MentorAvailableTimeSlots_TimeSlotId",
                table: "Sessions",
                column: "TimeSlotId",
                principalTable: "MentorAvailableTimeSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_MentorAvailableTimeSlots_TimeSlotId",
                table: "Sessions");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_MentorAvailableTimeSlots_TimeSlotId",
                table: "Sessions",
                column: "TimeSlotId",
                principalTable: "MentorAvailableTimeSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
