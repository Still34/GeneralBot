using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using GeneralBot.Models.Database.CoreSettings;
using Discord;

namespace GeneralBot.Migrations.Core
{
    [DbContext(typeof(CoreContext))]
    [Migration("20170809013713_MigrationRebuild")]
    partial class MigrationRebuild
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("GeneralBot.Models.Database.CoreSettings.ActivityLogging", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("GuildId");

                    b.Property<ulong>("LogChannel");

                    b.Property<bool>("ShouldLogJoin");

                    b.Property<bool>("ShouldLogLeave");

                    b.Property<bool>("ShouldLogVoice");

                    b.HasKey("Id");

                    b.ToTable("ActivityLogging");
                });

            modelBuilder.Entity("GeneralBot.Models.Database.CoreSettings.GreetingSettings", b =>
                {
                    b.Property<Guid>("Id")
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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CommandPrefix")
                        .IsRequired();

                    b.Property<ulong>("GuildId");

                    b.Property<bool>("IsGfyCatEnabled");

                    b.Property<bool>("IsInviteAllowed");

                    b.Property<byte>("ModeratorPermission");

                    b.Property<ulong>("ReportChannel");

                    b.HasKey("Id");

                    b.ToTable("GuildsSettings");
                });
        }
    }
}
