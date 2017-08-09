using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using GeneralBot.Models.Database.UserSettings;

namespace GeneralBot.Migrations
{
    [DbContext(typeof(UserContext))]
    [Migration("20170809013706_MigrationRebuild")]
    partial class MigrationRebuild
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("GeneralBot.Models.Context.Coordinate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<ulong>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Coordinates");
                });

            modelBuilder.Entity("GeneralBot.Models.Database.UserSettings.Games", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BattleTag");

                    b.Property<string>("NintendoFriendCode");

                    b.Property<string>("RiotId");

                    b.Property<ulong>("SteamId");

                    b.Property<ulong>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("GeneralBot.Models.Database.UserSettings.Profile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<uint>("Balance");

                    b.Property<DateTimeOffset>("LastMessage");

                    b.Property<string>("Summary")
                        .IsRequired();

                    b.Property<ulong>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("GeneralBot.Models.Database.UserSettings.Reminder", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("ChannelId");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(512);

                    b.Property<DateTimeOffset>("Time");

                    b.Property<ulong>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Reminders");
                });
        }
    }
}
