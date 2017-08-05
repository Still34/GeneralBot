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

        public ulong ReportChannel { get; set; } = 0;

        public bool IsInviteAllowed { get; set; } = true;

        public bool IsGfyCatEnabled { get; set; } = false;
        
        public GuildPermission ModeratorPermission { get; set; } = GuildPermission.KickMembers;
    }
}