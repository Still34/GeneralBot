using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralBot.Models.Database.UserSettings
{
    public class Games
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public ulong UserId { get; set; }

        public ulong SteamId { get; set; }

        public string BattleTag { get; set; }

        public string NintendoFriendCode { get; set; }
        
        public string RiotId { get; set; }
    }
}