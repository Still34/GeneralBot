using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GeneralBot.Common.Types
{
    public class Queue
    {
        public Queue(ulong guild)
        {
            Guild = guild;
        }

        public ulong Guild {get; set;}
        public List<Song> Songs {get; set;}
    }
}
