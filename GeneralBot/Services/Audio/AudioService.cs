using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using System.Collections.Generic;
using GeneralBot.Services;
using GeneralBot.Common.Types;

public class AudioService
{
    private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
    private readonly LoggingService _logger;

    public AudioService(LoggingService logger)
    { 
        _logger = logger;
    }


}