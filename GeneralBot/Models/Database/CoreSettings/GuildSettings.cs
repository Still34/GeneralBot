using System.ComponentModel.DataAnnotations;
using Discord;

namespace GeneralBot.Models.Database.CoreSettings
{
    public class GuildSettings
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public ulong GuildId { get; set; }

        [Required]
        public string CommandPrefix { get; set; } = "!";

        [Required]
        public bool WelcomeEnable { get; set; } = false;

        [Required]
        public ulong WelcomeChannel { get; set; }

        [Required]
        public string WelcomeMessage { get; set; } = "Welcome {mention} to the guild!";

        [Required]
        public GuildPermission ModeratorPermission { get; set; } = GuildPermission.KickMembers;
    }
}