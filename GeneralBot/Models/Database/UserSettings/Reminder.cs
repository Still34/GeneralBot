using System;
using System.ComponentModel.DataAnnotations;

namespace GeneralBot.Models.Database.UserSettings
{
    public class Reminder
    {
        [Key]
        public int Id { get; set; }

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