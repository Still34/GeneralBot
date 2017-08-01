using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GeneralBot.Results;
using YoutubeExplode;
using Humanizer;
using System.Linq;

namespace GeneralBot.Commands.User
{
    public class AudioModule : ModuleBase<SocketCommandContext>
    {
        private readonly AudioService _service;

        public AudioModule(AudioService service)
        {
            _service = service;
        }

    }
}
