using System;
using System.ComponentModel.DataAnnotations;

namespace GeneralBot.Models.Database.UserSettings
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public ulong UserId { get; set; }

        [Required]
        public uint Balance { get; set; } = 1;

        [Required]
        public DateTimeOffset LastMessage { get; set; } = DateTimeOffset.MinValue;

        [Required]
        public string Summary { get; set; } = "No summary.";
    }
}