using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpfulHive.Migrations
{
    /// <inheritdoc />
    public partial class HasViewedNewCommonRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasViewedNewCommonRecord",
                table: "UserPreferences",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasViewedNewCommonRecord",
                table: "UserPreferences");
        }
    }
}
