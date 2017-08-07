using System.ComponentModel.DataAnnotations;

namespace GeneralBot.Models.Database.UserSettings
{
    public class Games
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public ulong UserId { get; set; }

        public ulong SteamId { get; set; }

        public string BattleTag { get; set; }

        public string NintendoFriendCode { get; set; }
        
        public string RiotId { get; set; }
    }
}