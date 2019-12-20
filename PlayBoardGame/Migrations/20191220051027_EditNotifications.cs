using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayBoardGame.Migrations
{
    public partial class EditNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.CreateTable(
                name: "TomorrowsMeetingsNotifications",
                columns: table => new
                {
                    TomorrowsMeetingsNotificationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PostDate = table.Column<DateTime>(nullable: false),
                    MeetingStartDateTime = table.Column<DateTime>(nullable: false),
                    MeetingId = table.Column<int>(nullable: false),
                    ParticipantId = table.Column<string>(nullable: true),
                    NumberOfTries = table.Column<int>(nullable: false),
                    IfSent = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TomorrowsMeetingsNotifications", x => x.TomorrowsMeetingsNotificationId);
                    table.ForeignKey(
                        name: "FK_TomorrowsMeetingsNotifications_Meetings_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "Meetings",
                        principalColumn: "MeetingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TomorrowsMeetingsNotifications_AspNetUsers_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TomorrowsMeetingsNotifications_MeetingId",
                table: "TomorrowsMeetingsNotifications",
                column: "MeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_TomorrowsMeetingsNotifications_ParticipantId",
                table: "TomorrowsMeetingsNotifications",
                column: "ParticipantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TomorrowsMeetingsNotifications");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                });
        }
    }
}
