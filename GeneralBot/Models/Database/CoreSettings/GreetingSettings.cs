using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralBot.Models.Database.CoreSettings
{
    public class GreetingSettings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public ulong GuildId { get; set; }

        [Required]
        public bool IsJoinEnabled { get; set; } = false;

        public string WelcomeMessage { get; set; } = "Welcome {mention} to the guild!";

        public ulong ChannelId { get; set; } = 0;
    }
}