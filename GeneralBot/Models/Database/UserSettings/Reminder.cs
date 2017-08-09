using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralBot.Models.Database.UserSettings
{
    public class Reminder
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public DateTimeOffset Time { get; set; }

        [Required]
        public ulong UserId { get; set; }

        [Required]
        public ulong ChannelId { get; set; }

        [Required]
        [MaxLength(512)]
        public string Content { get; set; }
    }
}