using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgorkaCoins.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReportsWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModeratorComment",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReporterUsername",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ReviewedByUserId",
                table: "Reports",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedByUsername",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "StatusChangedAt",
                table: "Reports",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModeratorComment",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ReporterUsername",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ReviewedByUserId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ReviewedByUsername",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "StatusChangedAt",
                table: "Reports");
        }
    }
}
