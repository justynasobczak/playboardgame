using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayBoardGame.Migrations
{
    public partial class Addnotesinmeetings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Meetings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Meetings");
        }
    }
}
