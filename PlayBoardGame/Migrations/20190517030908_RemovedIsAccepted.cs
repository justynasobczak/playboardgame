using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayBoardGame.Migrations
{
    public partial class RemovedIsAccepted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "MeetingInvitedUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "MeetingInvitedUser",
                nullable: false,
                defaultValue: false);
        }
    }
}
