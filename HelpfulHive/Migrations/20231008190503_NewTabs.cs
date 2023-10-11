using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpfulHive.Migrations
{
    /// <inheritdoc />
    public partial class NewTabs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Uri",
                table: "Tabs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tabs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "ParentTabId",
                table: "Tabs",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tabs_ParentTabId",
                table: "Tabs",
                column: "ParentTabId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tabs_Tabs_ParentTabId",
                table: "Tabs",
                column: "ParentTabId",
                principalTable: "Tabs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tabs_Tabs_ParentTabId",
                table: "Tabs");

            migrationBuilder.DropIndex(
                name: "IX_Tabs_ParentTabId",
                table: "Tabs");

            migrationBuilder.DropColumn(
                name: "ParentTabId",
                table: "Tabs");

            migrationBuilder.AlterColumn<string>(
                name: "Uri",
                table: "Tabs",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tabs",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
