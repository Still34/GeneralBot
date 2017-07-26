using System.ComponentModel.DataAnnotations;
using Discord;
using Microsoft.EntityFrameworkCore;

namespace GeneralBot.Databases.Context
{
    public class CoreContext : DbContext
    {
        public DbSet<GuildSettings> GuildsSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite("Filename=./CoreSettings.db");
    }

    public class GuildSettings
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public ulong GuildId { get; set; }

        [Required]
        public string Prefix { get; set; } = "!";

        [Required]
        public GuildPermission ModeratorPermission { get; set; } = GuildPermission.KickMembers;
    }
}