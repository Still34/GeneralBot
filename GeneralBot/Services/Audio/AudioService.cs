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

    public async Task JoinChannel(IGuild guild, IVoiceChannel target)
    {
        IAudioClient client;
        if (ConnectedChannels.TryGetValue(guild.Id, out client))
            return;
        if (target.Guild.Id != guild.Id)
            return;

        var audioClient = await target.ConnectAsync();

        if (ConnectedChannels.TryAdd(guild.Id, audioClient))
        {
            await _logger.Log($"Connected to {guild.Name}({guild.Id})", LogSeverity.Info);
        }
    }

    public async Task LeaveGuild(IGuild guild)
    {
        IAudioClient client;
        if (ConnectedChannels.TryRemove(guild.Id, out client))
        {
            await client.StopAsync();
            await _logger.Log($"Disconnected from {guild.Name}({guild.Id})", LogSeverity.Info);
        }
    }

    private Process CreateStream(string streamUrl)
    {
        return Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg.exe",
            Arguments = $"-hide_banner -loglevel panic -i {streamUrl} -ac 2 -f s16le -ar 48000 pipe:1",
            UseShellExecute = false,
            RedirectStandardOutput = true
        });
    }
}