using System;
using System.ComponentModel.DataAnnotations;

namespace GeneralBot.Models.Database.CoreSettings.Poll
{
    public class PollData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public ulong GuildId { get; set; }

        /// <summary>
        /// Where should the result be sent to?
        /// </summary>
        public ulong ResultChannel { get; set; }

        /// <summary>
        /// Where is the poll being held?
        /// </summary>
        public ulong MainChannel { get; set; }

        /// <summary>
        /// Poll message itself.
        /// </summary>
        public ulong MessageId { get; set; }

        /// <summary>
        /// How should the poll be run?
        /// </summary>
        public PollMethod Method { get; set; }

        /// <summary>
        /// When does the poll end?
        /// </summary>
        public DateTimeOffset EndDate { get; set; }
    }
}