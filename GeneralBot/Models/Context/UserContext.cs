using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GeneralBot.Models.Context
{
    public class UserContext : DbContext
    {
        public DbSet<Coordinate> Coordinates { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite("Filename=./UserConfig.db");
    }

    public class Coordinate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public ulong UserId { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }
    }

    public class Reminder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTimeOffset Time { get; set; }

        [Required]
        public ulong UserId { get; set; }

        [Required]
        public ulong ChannelId { get; set; }

        [Required]
        [MaxLength(512)]
        public string Content { get; set; }
    }
}