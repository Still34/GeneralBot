using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GeneralBot.Common.Types
{
    public class Song
    {
        public Song(string name, TimeSpan duration, string url, CancellationTokenSource tokenSource)
        {
            Name = name;
            Duration = duration;
            StreamUrl = url;
            TokenSource = tokenSource;
        }

        public string Name { get; }

        public string StreamUrl { get; set; }

        public CancellationTokenSource TokenSource { get; set; }

        public CancellationToken Token => TokenSource.Token;

        public TimeSpan Duration { get; } 
    }
}
