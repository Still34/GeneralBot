using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GeneralBot.Migrations
{
    public partial class ReportChannelDefault : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildsSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommandPrefix = table.Column<string>(nullable: false),
                    GuildId = table.Column<ulong>(nullable: false),
                    ModeratorPermission = table.Column<byte>(nullable: false),
                    ReportChannel = table.Column<ulong>(nullable: false),
                    WelcomeChannel = table.Column<ulong>(nullable: false),
                    WelcomeEnable = table.Column<bool>(nullable: false),
                    WelcomeMessage = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildsSettings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildsSettings");
        }
    }
}
