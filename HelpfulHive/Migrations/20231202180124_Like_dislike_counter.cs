using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpfulHive.Migrations
{
    /// <inheritdoc />
    public partial class Like_dislike_counter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DislikesCount",
                table: "ProcessedDatas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LikesCount",
                table: "ProcessedDatas",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DislikesCount",
                table: "ProcessedDatas");

            migrationBuilder.DropColumn(
                name: "LikesCount",
                table: "ProcessedDatas");
        }
    }
}
