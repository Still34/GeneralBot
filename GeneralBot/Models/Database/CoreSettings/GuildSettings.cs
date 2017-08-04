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

        public bool AllowInvite { get; set; } = true;
        
        public GuildPermission ModeratorPermission { get; set; } = GuildPermission.KickMembers;
    }
}