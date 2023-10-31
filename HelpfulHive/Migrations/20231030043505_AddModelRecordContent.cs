using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HelpfulHive.Migrations
{
    /// <inheritdoc />
    public partial class AddModelRecordContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Records");

            migrationBuilder.AddColumn<int>(
                name: "ContentId",
                table: "Records",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RecordsContent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "text", nullable: false),
                    ImageUrls = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordsContent", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Records_ContentId",
                table: "Records",
                column: "ContentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_RecordsContent_ContentId",
                table: "Records",
                column: "ContentId",
                principalTable: "RecordsContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_RecordsContent_ContentId",
                table: "Records");

            migrationBuilder.DropTable(
                name: "RecordsContent");

            migrationBuilder.DropIndex(
                name: "IX_Records_ContentId",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "ContentId",
                table: "Records");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Records",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
