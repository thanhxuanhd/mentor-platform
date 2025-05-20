using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTableUsers_AddUserSettingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCategory_Categories_CategoryId",
                table: "UserCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCategory_Users_UserId",
                table: "UserCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTeachingApproach_TeachingApproach_TeachingApproachId",
                table: "UserTeachingApproach");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTeachingApproach_Users_UserId",
                table: "UserTeachingApproach");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTeachingApproach",
                table: "UserTeachingApproach");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCategory",
                table: "UserCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeachingApproach",
                table: "TeachingApproach");

            migrationBuilder.RenameTable(
                name: "UserTeachingApproach",
                newName: "UserTeachingApproaches");

            migrationBuilder.RenameTable(
                name: "UserCategory",
                newName: "UserCategories");

            migrationBuilder.RenameTable(
                name: "TeachingApproach",
                newName: "TeachingApproaches");

            migrationBuilder.RenameIndex(
                name: "IX_UserTeachingApproach_UserId_TeachingApproachId",
                table: "UserTeachingApproaches",
                newName: "IX_UserTeachingApproaches_UserId_TeachingApproachId");

            migrationBuilder.RenameIndex(
                name: "IX_UserTeachingApproach_TeachingApproachId",
                table: "UserTeachingApproaches",
                newName: "IX_UserTeachingApproaches_TeachingApproachId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCategory_UserId_CategoryId",
                table: "UserCategories",
                newName: "IX_UserCategories_UserId_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCategory_CategoryId",
                table: "UserCategories",
                newName: "IX_UserCategories_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_TeachingApproach_Name",
                table: "TeachingApproaches",
                newName: "IX_TeachingApproaches_Name");

            migrationBuilder.AddColumn<bool>(
                name: "IsAllowedMessage",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReceiveNotification",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTeachingApproaches",
                table: "UserTeachingApproaches",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCategories",
                table: "UserCategories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeachingApproaches",
                table: "TeachingApproaches",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCategories_Categories_CategoryId",
                table: "UserCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCategories_Users_UserId",
                table: "UserCategories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTeachingApproaches_TeachingApproaches_TeachingApproachId",
                table: "UserTeachingApproaches",
                column: "TeachingApproachId",
                principalTable: "TeachingApproaches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTeachingApproaches_Users_UserId",
                table: "UserTeachingApproaches",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCategories_Categories_CategoryId",
                table: "UserCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCategories_Users_UserId",
                table: "UserCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTeachingApproaches_TeachingApproaches_TeachingApproachId",
                table: "UserTeachingApproaches");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTeachingApproaches_Users_UserId",
                table: "UserTeachingApproaches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTeachingApproaches",
                table: "UserTeachingApproaches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCategories",
                table: "UserCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeachingApproaches",
                table: "TeachingApproaches");

            migrationBuilder.DropColumn(
                name: "IsAllowedMessage",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsReceiveNotification",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "UserTeachingApproaches",
                newName: "UserTeachingApproach");

            migrationBuilder.RenameTable(
                name: "UserCategories",
                newName: "UserCategory");

            migrationBuilder.RenameTable(
                name: "TeachingApproaches",
                newName: "TeachingApproach");

            migrationBuilder.RenameIndex(
                name: "IX_UserTeachingApproaches_UserId_TeachingApproachId",
                table: "UserTeachingApproach",
                newName: "IX_UserTeachingApproach_UserId_TeachingApproachId");

            migrationBuilder.RenameIndex(
                name: "IX_UserTeachingApproaches_TeachingApproachId",
                table: "UserTeachingApproach",
                newName: "IX_UserTeachingApproach_TeachingApproachId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCategories_UserId_CategoryId",
                table: "UserCategory",
                newName: "IX_UserCategory_UserId_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCategories_CategoryId",
                table: "UserCategory",
                newName: "IX_UserCategory_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_TeachingApproaches_Name",
                table: "TeachingApproach",
                newName: "IX_TeachingApproach_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTeachingApproach",
                table: "UserTeachingApproach",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCategory",
                table: "UserCategory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeachingApproach",
                table: "TeachingApproach",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCategory_Categories_CategoryId",
                table: "UserCategory",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCategory_Users_UserId",
                table: "UserCategory",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTeachingApproach_TeachingApproach_TeachingApproachId",
                table: "UserTeachingApproach",
                column: "TeachingApproachId",
                principalTable: "TeachingApproach",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTeachingApproach_Users_UserId",
                table: "UserTeachingApproach",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
