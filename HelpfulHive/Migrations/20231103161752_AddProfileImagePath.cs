using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpfulHive.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrls",
                table: "RecordsContent",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text[]");

            migrationBuilder.AddColumn<string>(
                name: "ProfileImagePath",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImagePath",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrls",
                table: "RecordsContent",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb");
        }
    }
}
