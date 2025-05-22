using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTableAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Availability",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "Availabilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Availabilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAvailabilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AvailabilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAvailabilities_Availabilities_AvailabilityId",
                        column: x => x.AvailabilityId,
                        principalTable: "Availabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAvailabilities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAvailabilities_AvailabilityId",
                table: "UserAvailabilities",
                column: "AvailabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAvailabilities_UserId_AvailabilityId",
                table: "UserAvailabilities",
                columns: new[] { "UserId", "AvailabilityId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAvailabilities");

            migrationBuilder.DropTable(
                name: "Availabilities");

            migrationBuilder.AddColumn<string>(
                name: "Availability",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
