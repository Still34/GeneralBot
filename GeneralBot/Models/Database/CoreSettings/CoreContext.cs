using Microsoft.EntityFrameworkCore;

namespace GeneralBot.Models.Database.CoreSettings
{
    public class CoreContext : DbContext
    {
        public CoreContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ActivityLogging> ActivityLogging { get; set; }
        public DbSet<GreetingSettings> GreetingsSettings { get; set; }
        public DbSet<GuildSettings> GuildsSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Filename=./CoreSettings.db");
        }
    }
}