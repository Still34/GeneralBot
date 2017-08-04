using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GeneralBot.Migrations
{
    public partial class WelcomeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GreetingsSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
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
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AllowInvite = table.Column<bool>(nullable: false),
                    CommandPrefix = table.Column<string>(nullable: false),
                    GuildId = table.Column<ulong>(nullable: false),
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
                name: "GreetingsSettings");

            migrationBuilder.DropTable(
                name: "GuildsSettings");
        }
    }
}
