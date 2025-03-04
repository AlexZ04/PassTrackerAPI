using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PassTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class changedReq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "requests",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "requests");
        }
    }
}
