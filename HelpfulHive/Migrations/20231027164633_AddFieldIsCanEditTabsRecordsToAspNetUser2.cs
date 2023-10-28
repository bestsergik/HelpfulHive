﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpfulHive.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldIsCanEditTabsRecordsToAspNetUser2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCanEditTabsRecords",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCanEditTabsRecords",
                table: "AspNetUsers");
        }
    }
}