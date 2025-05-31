using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataMentorApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationDocument_MentorApplication_MentorApplicationId",
                table: "ApplicationDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_MentorApplication_Users_MentorId",
                table: "MentorApplication");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MentorApplication",
                table: "MentorApplication");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationDocument",
                table: "ApplicationDocument");

            migrationBuilder.RenameTable(
                name: "MentorApplication",
                newName: "MentorApplications");

            migrationBuilder.RenameTable(
                name: "ApplicationDocument",
                newName: "ApplicationDocuments");

            migrationBuilder.RenameIndex(
                name: "IX_MentorApplication_MentorId",
                table: "MentorApplications",
                newName: "IX_MentorApplications_MentorId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationDocument_MentorApplicationId",
                table: "ApplicationDocuments",
                newName: "IX_ApplicationDocuments_MentorApplicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MentorApplications",
                table: "MentorApplications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationDocuments",
                table: "ApplicationDocuments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationDocuments_MentorApplications_MentorApplicationId",
                table: "ApplicationDocuments",
                column: "MentorApplicationId",
                principalTable: "MentorApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MentorApplications_Users_MentorId",
                table: "MentorApplications",
                column: "MentorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationDocuments_MentorApplications_MentorApplicationId",
                table: "ApplicationDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_MentorApplications_Users_MentorId",
                table: "MentorApplications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MentorApplications",
                table: "MentorApplications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationDocuments",
                table: "ApplicationDocuments");

            migrationBuilder.RenameTable(
                name: "MentorApplications",
                newName: "MentorApplication");

            migrationBuilder.RenameTable(
                name: "ApplicationDocuments",
                newName: "ApplicationDocument");

            migrationBuilder.RenameIndex(
                name: "IX_MentorApplications_MentorId",
                table: "MentorApplication",
                newName: "IX_MentorApplication_MentorId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationDocuments_MentorApplicationId",
                table: "ApplicationDocument",
                newName: "IX_ApplicationDocument_MentorApplicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MentorApplication",
                table: "MentorApplication",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationDocument",
                table: "ApplicationDocument",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationDocument_MentorApplication_MentorApplicationId",
                table: "ApplicationDocument",
                column: "MentorApplicationId",
                principalTable: "MentorApplication",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MentorApplication_Users_MentorId",
                table: "MentorApplication",
                column: "MentorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
