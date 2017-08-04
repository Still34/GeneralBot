using System.ComponentModel.DataAnnotations;

namespace GeneralBot.Models.Database.CoreSettings
{
    public class GreetingSettings
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public ulong GuildId { get; set; }

        [Required]
        public bool IsJoinEnabled { get; set; } = false;

        public string WelcomeMessage { get; set; } = "Welcome {mention} to the guild!";

        public ulong ChannelId { get; set; } = 0;
    }
}