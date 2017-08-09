using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralBot.Models.Database.CoreSettings
{
    public class ActivityLogging
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public ulong GuildId { get; set; }

        public ulong LogChannel { get; set; } = 0;
        public bool ShouldLogJoin { get; set; } = false;
        public bool ShouldLogLeave { get; set; } = false;
        public bool ShouldLogVoice { get; set; } = false;
    }
}