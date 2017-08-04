using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using GeneralBot.Models.Database.UserSettings;

namespace GeneralBot.Migrations.User
{
    [DbContext(typeof(UserContext))]
    [Migration("20170804152720_Reminder")]
    partial class Reminder
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("GeneralBot.Models.Context.Coordinate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<ulong>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Coordinates");
                });

            modelBuilder.Entity("GeneralBot.Models.Database.UserSettings.Profile", b =>
                {
                    b.Property<int>("Id")
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
                    b.Property<int>("Id")
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
