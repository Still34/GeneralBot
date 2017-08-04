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

        public ulong WelcomeChannel { get; set; } = 0;

        [Required]
        public string WelcomeMessage { get; set; } = "Welcome {mention} to the guild!";

        public ulong ReportChannel { get; set; } = 0;

        public bool AllowInvite { get; set; }

        [Required]
        public GuildPermission ModeratorPermission { get; set; } = GuildPermission.KickMembers;
    }
}