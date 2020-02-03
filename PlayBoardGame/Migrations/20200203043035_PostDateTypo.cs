using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayBoardGame.Migrations
{
    public partial class PostDateTypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostStartDateTime",
                table: "FriendInvitations");

            migrationBuilder.AddColumn<DateTime>(
                name: "PostDateTime",
                table: "FriendInvitations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostDateTime",
                table: "FriendInvitations");

            migrationBuilder.AddColumn<DateTime>(
                name: "PostStartDateTime",
                table: "FriendInvitations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
