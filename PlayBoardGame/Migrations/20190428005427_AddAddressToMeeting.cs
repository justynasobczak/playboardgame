using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayBoardGame.Migrations
{
    public partial class AddAddressToMeeting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Meetings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Meetings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Meetings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Meetings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Meetings");
        }
    }
}
