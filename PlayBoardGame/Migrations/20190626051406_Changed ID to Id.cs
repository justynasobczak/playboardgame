using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayBoardGame.Migrations
{
    public partial class ChangedIDtoId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameAppUser_Games_GameID",
                table: "GameAppUser");

            migrationBuilder.DropForeignKey(
                name: "FK_MeetingGame_Games_GameID",
                table: "MeetingGame");

            migrationBuilder.DropForeignKey(
                name: "FK_MeetingGame_Meetings_MeetingID",
                table: "MeetingGame");

            migrationBuilder.DropForeignKey(
                name: "FK_MeetingInvitedUser_Meetings_MeetingID",
                table: "MeetingInvitedUser");

            migrationBuilder.RenameColumn(
                name: "MeetingID",
                table: "Meetings",
                newName: "MeetingId");

            migrationBuilder.RenameColumn(
                name: "MeetingID",
                table: "MeetingInvitedUser",
                newName: "MeetingId");

            migrationBuilder.RenameColumn(
                name: "MeetingID",
                table: "MeetingGame",
                newName: "MeetingId");

            migrationBuilder.RenameColumn(
                name: "GameID",
                table: "MeetingGame",
                newName: "GameId");

            migrationBuilder.RenameIndex(
                name: "IX_MeetingGame_MeetingID",
                table: "MeetingGame",
                newName: "IX_MeetingGame_MeetingId");

            migrationBuilder.RenameColumn(
                name: "GameID",
                table: "Games",
                newName: "GameId");

            migrationBuilder.RenameColumn(
                name: "GameID",
                table: "GameAppUser",
                newName: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameAppUser_Games_GameId",
                table: "GameAppUser",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MeetingGame_Games_GameId",
                table: "MeetingGame",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MeetingGame_Meetings_MeetingId",
                table: "MeetingGame",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "MeetingId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MeetingInvitedUser_Meetings_MeetingId",
                table: "MeetingInvitedUser",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "MeetingId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameAppUser_Games_GameId",
                table: "GameAppUser");

            migrationBuilder.DropForeignKey(
                name: "FK_MeetingGame_Games_GameId",
                table: "MeetingGame");

            migrationBuilder.DropForeignKey(
                name: "FK_MeetingGame_Meetings_MeetingId",
                table: "MeetingGame");

            migrationBuilder.DropForeignKey(
                name: "FK_MeetingInvitedUser_Meetings_MeetingId",
                table: "MeetingInvitedUser");

            migrationBuilder.RenameColumn(
                name: "MeetingId",
                table: "Meetings",
                newName: "MeetingID");

            migrationBuilder.RenameColumn(
                name: "MeetingId",
                table: "MeetingInvitedUser",
                newName: "MeetingID");

            migrationBuilder.RenameColumn(
                name: "MeetingId",
                table: "MeetingGame",
                newName: "MeetingID");

            migrationBuilder.RenameColumn(
                name: "GameId",
                table: "MeetingGame",
                newName: "GameID");

            migrationBuilder.RenameIndex(
                name: "IX_MeetingGame_MeetingId",
                table: "MeetingGame",
                newName: "IX_MeetingGame_MeetingID");

            migrationBuilder.RenameColumn(
                name: "GameId",
                table: "Games",
                newName: "GameID");

            migrationBuilder.RenameColumn(
                name: "GameId",
                table: "GameAppUser",
                newName: "GameID");

            migrationBuilder.AddForeignKey(
                name: "FK_GameAppUser_Games_GameID",
                table: "GameAppUser",
                column: "GameID",
                principalTable: "Games",
                principalColumn: "GameID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MeetingGame_Games_GameID",
                table: "MeetingGame",
                column: "GameID",
                principalTable: "Games",
                principalColumn: "GameID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MeetingGame_Meetings_MeetingID",
                table: "MeetingGame",
                column: "MeetingID",
                principalTable: "Meetings",
                principalColumn: "MeetingID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MeetingInvitedUser_Meetings_MeetingID",
                table: "MeetingInvitedUser",
                column: "MeetingID",
                principalTable: "Meetings",
                principalColumn: "MeetingID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
