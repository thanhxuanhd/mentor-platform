using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUser_CourseNavigationProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Users_MentorId",
                table: "Courses");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Users_MentorId",
                table: "Courses",
                column: "MentorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Users_MentorId",
                table: "Courses");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Users_MentorId",
                table: "Courses",
                column: "MentorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
