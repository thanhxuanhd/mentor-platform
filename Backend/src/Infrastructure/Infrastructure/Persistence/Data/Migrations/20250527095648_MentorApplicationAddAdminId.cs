using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class MentorApplicationAddAdminId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MentorApplications_Users_MentorId",
                table: "MentorApplications");

            migrationBuilder.AddColumn<Guid>(
                name: "AdminId",
                table: "MentorApplications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MentorApplications_AdminId",
                table: "MentorApplications",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_MentorApplications_Users_AdminId",
                table: "MentorApplications",
                column: "AdminId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MentorApplications_Users_MentorId",
                table: "MentorApplications",
                column: "MentorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MentorApplications_Users_AdminId",
                table: "MentorApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_MentorApplications_Users_MentorId",
                table: "MentorApplications");

            migrationBuilder.DropIndex(
                name: "IX_MentorApplications_AdminId",
                table: "MentorApplications");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "MentorApplications");

            migrationBuilder.AddForeignKey(
                name: "FK_MentorApplications_Users_MentorId",
                table: "MentorApplications",
                column: "MentorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
