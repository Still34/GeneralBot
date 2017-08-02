using Microsoft.EntityFrameworkCore;

namespace GeneralBot.Models.Database.CoreSettings
{
    public class CoreContext : DbContext
    {
        public DbSet<GuildSettings> GuildsSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite("Filename=./CoreSettings.db");
    }
}