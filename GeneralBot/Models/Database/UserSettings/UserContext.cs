using Microsoft.EntityFrameworkCore;

namespace GeneralBot.Models.Database.UserSettings
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        public DbSet<Coordinate> Coordinates { get; set; }
        public DbSet<Games> Games { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Reminder> Reminders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite(
                "Filename=./UserConfig.db");
        }
    }
}