using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using GeneralBot.Services;

public class AudioService
{
    private readonly LoggingService _logger;

    private readonly ConcurrentDictionary<ulong, IAudioClient> _connectedChannels =
        new ConcurrentDictionary<ulong, IAudioClient>();

    public AudioService(LoggingService logger) => _logger = logger;

    public async Task JoinChannelAsync(IGuild guild, IVoiceChannel target)
    {
        IAudioClient client;
        if (_connectedChannels.TryGetValue(guild.Id, out client))
            return;
        if (target.Guild.Id != guild.Id)
            return;

        var audioClient = await target.ConnectAsync();

        if (_connectedChannels.TryAdd(guild.Id, audioClient))
            await _logger.LogAsync($"Connected to {guild.Name}({guild.Id})", LogSeverity.Info);
    }

    public async Task LeaveGuildAsync(IGuild guild)
    {
        IAudioClient client;
        if (_connectedChannels.TryRemove(guild.Id, out client))
        {
            await client.StopAsync();
            await _logger.LogAsync($"Disconnected from {guild.Name}({guild.Id})", LogSeverity.Info);
        }
    }

    private Process CreateStream(string streamUrl) => Process.Start(new ProcessStartInfo
    {
        FileName = "ffmpeg.exe",
        Arguments = $"-hide_banner -loglevel panic -i {streamUrl} -ac 2 -f s16le -ar 48000 pipe:1",
        UseShellExecute = false,
        RedirectStandardOutput = true
    });
}