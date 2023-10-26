using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpfulHive.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsTORecordModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_Tabs_SubTabId",
                table: "Records");

            migrationBuilder.AddColumn<int>(
                name: "ClickCount",
                table: "Records",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "Records",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Tabs_SubTabId",
                table: "Records",
                column: "SubTabId",
                principalTable: "Tabs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_Tabs_SubTabId",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "ClickCount",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "Records");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Tabs_SubTabId",
                table: "Records",
                column: "SubTabId",
                principalTable: "Tabs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
