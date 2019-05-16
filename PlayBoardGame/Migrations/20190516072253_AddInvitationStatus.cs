using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayBoardGame.Migrations
{
    public partial class AddInvitationStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "MeetingInvitedUser",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "MeetingInvitedUser");
        }
    }
}
