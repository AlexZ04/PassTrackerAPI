using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PassTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class InDeaneryReqField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "InDeanery",
                table: "requests",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InDeanery",
                table: "requests");
        }
    }
}
