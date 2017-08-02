using System.ComponentModel.DataAnnotations;

namespace GeneralBot.Models.Context
{
    public class Coordinate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public ulong UserId { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }
    }
}