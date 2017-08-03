using System.Collections.Generic;
using System.Linq;

namespace GeneralBot.Common.Types
{
    public class Queue
    {
        public Queue(ulong guild) => Guild = guild;

        public ulong Guild { get; set; }
        public List<Song> Songs { get; set; }

        public void AddSong(Song song) => Songs.Add(song);

        public void RemoveSong(Song song) => Songs.Remove(song);

        public void RemoveSong(string streamUrl) => Songs.Remove(Songs.SingleOrDefault(x => x.StreamUrl == streamUrl));
    }
}