using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTableExpertise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserExpertise_Expertise_ExpertiseId",
                table: "UserExpertise");

            migrationBuilder.DropForeignKey(
                name: "FK_UserExpertise_Users_UserId",
                table: "UserExpertise");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserExpertise",
                table: "UserExpertise");

            migrationBuilder.DropIndex(
                name: "IX_UserExpertise_UserId",
                table: "UserExpertise");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Expertise",
                table: "Expertise");

            migrationBuilder.RenameTable(
                name: "UserExpertise",
                newName: "UserExpertises");

            migrationBuilder.RenameTable(
                name: "Expertise",
                newName: "Expertises");

            migrationBuilder.RenameIndex(
                name: "IX_UserExpertise_ExpertiseId",
                table: "UserExpertises",
                newName: "IX_UserExpertises_ExpertiseId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Expertises",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserExpertises",
                table: "UserExpertises",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Expertises",
                table: "Expertises",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserExpertises_UserId_ExpertiseId",
                table: "UserExpertises",
                columns: new[] { "UserId", "ExpertiseId" });

            migrationBuilder.CreateIndex(
                name: "IX_Expertises_Name",
                table: "Expertises",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserExpertises_Expertises_ExpertiseId",
                table: "UserExpertises",
                column: "ExpertiseId",
                principalTable: "Expertises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserExpertises_Users_UserId",
                table: "UserExpertises",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserExpertises_Expertises_ExpertiseId",
                table: "UserExpertises");

            migrationBuilder.DropForeignKey(
                name: "FK_UserExpertises_Users_UserId",
                table: "UserExpertises");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserExpertises",
                table: "UserExpertises");

            migrationBuilder.DropIndex(
                name: "IX_UserExpertises_UserId_ExpertiseId",
                table: "UserExpertises");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Expertises",
                table: "Expertises");

            migrationBuilder.DropIndex(
                name: "IX_Expertises_Name",
                table: "Expertises");

            migrationBuilder.RenameTable(
                name: "UserExpertises",
                newName: "UserExpertise");

            migrationBuilder.RenameTable(
                name: "Expertises",
                newName: "Expertise");

            migrationBuilder.RenameIndex(
                name: "IX_UserExpertises_ExpertiseId",
                table: "UserExpertise",
                newName: "IX_UserExpertise_ExpertiseId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Expertise",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserExpertise",
                table: "UserExpertise",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Expertise",
                table: "Expertise",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserExpertise_UserId",
                table: "UserExpertise",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserExpertise_Expertise_ExpertiseId",
                table: "UserExpertise",
                column: "ExpertiseId",
                principalTable: "Expertise",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserExpertise_Users_UserId",
                table: "UserExpertise",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
