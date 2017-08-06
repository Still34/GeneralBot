using GeneralBot.Models.Database.CoreSettings.Poll;
using Microsoft.EntityFrameworkCore;

namespace GeneralBot.Models.Database.CoreSettings
{
    public class CoreContext : DbContext
    {
        public DbSet<GuildSettings> GuildsSettings { get; set; }
        public DbSet<GreetingSettings> GreetingsSettings { get; set; }
        public DbSet<ActivityLogging> ActivityLogging { get; set; }
        public DbSet<PollData> PollsData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite("Filename=./CoreSettings.db");
    }
}