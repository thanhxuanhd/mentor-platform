using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTableUsers_AddNewFieldsAndJunctionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PreferredCommunicationMethod",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "VideoCall",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Goal",
                table: "Users",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredLearningStyle",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Visual");

            migrationBuilder.AddColumn<int>(
                name: "PreferredSessionDuration",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 30);

            migrationBuilder.AddColumn<string>(
                name: "PreferredSessionFrequency",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "AsNeeded");

            migrationBuilder.CreateTable(
                name: "TeachingApproach",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeachingApproach", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCategory_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCategory_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTeachingApproach",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeachingApproachId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTeachingApproach", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTeachingApproach_TeachingApproach_TeachingApproachId",
                        column: x => x.TeachingApproachId,
                        principalTable: "TeachingApproach",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTeachingApproach_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_PreferredSessionDuration",
                table: "Users",
                sql: "\"PreferredSessionDuration\" IN (30, 45, 60, 90, 120)");

            migrationBuilder.CreateIndex(
                name: "IX_TeachingApproach_Name",
                table: "TeachingApproach",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCategory_CategoryId",
                table: "UserCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCategory_UserId_CategoryId",
                table: "UserCategory",
                columns: new[] { "UserId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserTeachingApproach_TeachingApproachId",
                table: "UserTeachingApproach",
                column: "TeachingApproachId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTeachingApproach_UserId_TeachingApproachId",
                table: "UserTeachingApproach",
                columns: new[] { "UserId", "TeachingApproachId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCategory");

            migrationBuilder.DropTable(
                name: "UserTeachingApproach");

            migrationBuilder.DropTable(
                name: "TeachingApproach");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PreferredSessionDuration",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PreferredLearningStyle",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PreferredSessionDuration",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PreferredSessionFrequency",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "PreferredCommunicationMethod",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "VideoCall");

            migrationBuilder.AlterColumn<string>(
                name: "Goal",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
