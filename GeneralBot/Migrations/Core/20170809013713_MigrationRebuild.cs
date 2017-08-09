using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GeneralBot.Migrations.Core
{
    public partial class MigrationRebuild : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityLogging",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    GuildId = table.Column<ulong>(nullable: false),
                    LogChannel = table.Column<ulong>(nullable: false),
                    ShouldLogJoin = table.Column<bool>(nullable: false),
                    ShouldLogLeave = table.Column<bool>(nullable: false),
                    ShouldLogVoice = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogging", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GreetingsSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ChannelId = table.Column<ulong>(nullable: false),
                    GuildId = table.Column<ulong>(nullable: false),
                    IsJoinEnabled = table.Column<bool>(nullable: false),
                    WelcomeMessage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GreetingsSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuildsSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CommandPrefix = table.Column<string>(nullable: false),
                    GuildId = table.Column<ulong>(nullable: false),
                    IsGfyCatEnabled = table.Column<bool>(nullable: false),
                    IsInviteAllowed = table.Column<bool>(nullable: false),
                    ModeratorPermission = table.Column<byte>(nullable: false),
                    ReportChannel = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildsSettings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLogging");

            migrationBuilder.DropTable(
                name: "GreetingsSettings");

            migrationBuilder.DropTable(
                name: "GuildsSettings");
        }
    }
}
