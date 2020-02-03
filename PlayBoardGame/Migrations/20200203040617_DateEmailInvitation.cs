using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayBoardGame.Migrations
{
    public partial class DateEmailInvitation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvitedEmail",
                table: "FriendInvitations",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PostStartDateTime",
                table: "FriendInvitations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvitedEmail",
                table: "FriendInvitations");

            migrationBuilder.DropColumn(
                name: "PostStartDateTime",
                table: "FriendInvitations");
        }
    }
}
