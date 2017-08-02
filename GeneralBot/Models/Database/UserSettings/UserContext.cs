using GeneralBot.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace GeneralBot.Models.Database.UserSettings
{
    public class UserContext : DbContext
    {
        public DbSet<Coordinate> Coordinates { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Reminder> Reminders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite(
            "Filename=./UserConfig.db");
    }
}