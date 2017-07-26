using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using GeneralBot.Databases.Context;

namespace GeneralBot.Migrations
{
    [DbContext(typeof(CoreContext))]
    [Migration("20170726153209_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("GeneralBot.Databases.Context.GuildSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("GuildId");

                    b.Property<string>("Prefix")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("GuildsSettings");
                });
        }
    }
}
