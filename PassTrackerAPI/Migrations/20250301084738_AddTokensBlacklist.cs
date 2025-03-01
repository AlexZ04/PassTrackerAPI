using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PassTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddTokensBlacklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "blacklistTokens",
                columns: table => new
                {
                    Token = table.Column<string>(type: "text", nullable: false),
                    AddTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blacklistTokens", x => x.Token);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blacklistTokens");
        }
    }
}
