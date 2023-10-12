using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpfulHive.Migrations
{
    /// <inheritdoc />
    public partial class AddTabs2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Tabs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Tabs");
        }
    }
}
