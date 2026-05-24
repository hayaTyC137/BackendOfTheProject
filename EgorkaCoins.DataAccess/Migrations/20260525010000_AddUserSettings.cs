using EgorkaCoins.DataAccess.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EgorkaCoins.DataAccess.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260525010000_AddUserSettings")]
    public partial class AddUserSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "NotifyOrders",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotifyPromo",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotifySecurity",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NotifyOrders",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NotifyPromo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NotifySecurity",
                table: "Users");
        }
    }
}
