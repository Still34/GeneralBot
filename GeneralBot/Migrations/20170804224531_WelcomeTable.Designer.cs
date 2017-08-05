using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using GeneralBot.Models.Database.CoreSettings;
using Discord;

namespace GeneralBot.Migrations
{
    [DbContext(typeof(CoreContext))]
    [Migration("20170804224531_WelcomeTable")]
    partial class WelcomeTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("GeneralBot.Models.Database.CoreSettings.GreetingSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("ChannelId");

                    b.Property<ulong>("GuildId");

                    b.Property<bool>("IsJoinEnabled");

                    b.Property<string>("WelcomeMessage");

                    b.HasKey("Id");

                    b.ToTable("GreetingsSettings");
                });

            modelBuilder.Entity("GeneralBot.Models.Database.CoreSettings.GuildSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsInviteAllowed");

                    b.Property<string>("CommandPrefix")
                        .IsRequired();

                    b.Property<ulong>("GuildId");

                    b.Property<byte>("ModeratorPermission");

                    b.Property<ulong>("ReportChannel");

                    b.HasKey("Id");

                    b.ToTable("GuildsSettings");
                });
        }
    }
}
