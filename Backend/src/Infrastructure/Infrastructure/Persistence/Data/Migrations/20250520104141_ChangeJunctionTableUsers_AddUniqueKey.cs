using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeJunctionTableUsers_AddUniqueKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserTeachingApproaches_UserId_TeachingApproachId",
                table: "UserTeachingApproaches");

            migrationBuilder.DropIndex(
                name: "IX_UserExpertises_UserId_ExpertiseId",
                table: "UserExpertises");

            migrationBuilder.DropIndex(
                name: "IX_UserCategories_UserId_CategoryId",
                table: "UserCategories");

            migrationBuilder.DropIndex(
                name: "IX_UserAvailabilities_UserId_AvailabilityId",
                table: "UserAvailabilities");

            migrationBuilder.CreateIndex(
                name: "IX_UserTeachingApproaches_UserId_TeachingApproachId",
                table: "UserTeachingApproaches",
                columns: new[] { "UserId", "TeachingApproachId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserExpertises_UserId_ExpertiseId",
                table: "UserExpertises",
                columns: new[] { "UserId", "ExpertiseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCategories_UserId_CategoryId",
                table: "UserCategories",
                columns: new[] { "UserId", "CategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAvailabilities_UserId_AvailabilityId",
                table: "UserAvailabilities",
                columns: new[] { "UserId", "AvailabilityId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserTeachingApproaches_UserId_TeachingApproachId",
                table: "UserTeachingApproaches");

            migrationBuilder.DropIndex(
                name: "IX_UserExpertises_UserId_ExpertiseId",
                table: "UserExpertises");

            migrationBuilder.DropIndex(
                name: "IX_UserCategories_UserId_CategoryId",
                table: "UserCategories");

            migrationBuilder.DropIndex(
                name: "IX_UserAvailabilities_UserId_AvailabilityId",
                table: "UserAvailabilities");

            migrationBuilder.CreateIndex(
                name: "IX_UserTeachingApproaches_UserId_TeachingApproachId",
                table: "UserTeachingApproaches",
                columns: new[] { "UserId", "TeachingApproachId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserExpertises_UserId_ExpertiseId",
                table: "UserExpertises",
                columns: new[] { "UserId", "ExpertiseId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserCategories_UserId_CategoryId",
                table: "UserCategories",
                columns: new[] { "UserId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserAvailabilities_UserId_AvailabilityId",
                table: "UserAvailabilities",
                columns: new[] { "UserId", "AvailabilityId" });
        }
    }
}
