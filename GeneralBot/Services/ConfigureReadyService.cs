using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace GeneralBot.Services
{
    /// <summary>
    ///     Reports to log whenever the client is ready.
    /// </summary>
    internal class ConfigureReadyService
    {
        private readonly DiscordSocketClient _client;
        private readonly LoggingService _loggingService;

        public ConfigureReadyService(DiscordSocketClient client, LoggingService loggingService)
        {
            _client = client;
            _loggingService = loggingService;
            _client.Ready += ReadyReportAsync;
        }

        private async Task ReadyReportAsync()
        {
            var guilds = _client.Guilds.ToList();
            var sb = new StringBuilder();
            sb.AppendLine($"Connected as {_client.CurrentUser} ({_client.CurrentUser.Id}).");
            sb.AppendLine(
                $"Connected to {guilds.Count} server(s):");
            foreach (var guild in guilds)
            {
                sb.AppendLine("|");
                sb.AppendLine($"|----{guild.Name} ({guild.Id})");
                sb.AppendLine($"|--------User Count: {guild.MemberCount}");
                sb.AppendLine($"|--------Joined at: {guild.CurrentUser.JoinedAt}");
            }
            await _loggingService.LogAsync(sb, LogSeverity.Info).ConfigureAwait(false);
        }
    }
}