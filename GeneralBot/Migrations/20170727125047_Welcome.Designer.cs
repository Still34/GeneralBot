using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Discord;
using GeneralBot.Models.Context;

namespace GeneralBot.Migrations
{
    [DbContext(typeof(CoreContext))]
    [Migration("20170727125047_Welcome")]
    partial class Welcome
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("GeneralBot.Databases.Context.GuildSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CommandPrefix")
                        .IsRequired();

                    b.Property<ulong>("GuildId");

                    b.Property<byte>("ModeratorPermission");

                    b.Property<ulong>("WelcomeChannel");

                    b.Property<bool>("WelcomeEnable");

                    b.Property<string>("WelcomeMessage")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("GuildsSettings");
                });
        }
    }
}
