using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayBoardGame.Migrations
{
    public partial class OrganizerForMeeting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizerId",
                table: "Meetings",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_OrganizerId",
                table: "Meetings",
                column: "OrganizerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_AspNetUsers_OrganizerId",
                table: "Meetings",
                column: "OrganizerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_AspNetUsers_OrganizerId",
                table: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_Meetings_OrganizerId",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "OrganizerId",
                table: "Meetings");
        }
    }
}
