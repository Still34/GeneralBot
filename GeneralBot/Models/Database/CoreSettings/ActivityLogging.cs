using System.ComponentModel.DataAnnotations;

namespace GeneralBot.Models.Database.CoreSettings
{
    public class ActivityLogging
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public ulong GuildId { get; set; }

        public ulong LogChannel { get; set; } = 0;
        public bool ShouldLogJoin { get; set; } = false;
        public bool ShouldLogLeave { get; set; } = false;
        public bool ShouldLogVoice { get; set; } = false;
    }
}