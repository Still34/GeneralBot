using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace GeneralBot.Services
{
    /// <summary>
    ///     Configures the presence dynamically according to the bot's latency.
    /// </summary>
    internal class ConfigurePresenceService
    {
        private readonly DiscordSocketClient _client;

        public ConfigurePresenceService(DiscordSocketClient client)
        {
            _client = client;
            _client.LatencyUpdated += LatencyUpdatedAsync;
        }

        private async Task LatencyUpdatedAsync(int latencyBefore, int latencyAfter)
        {
            var currentStatus = _client.CurrentUser.Status;
            var newStatus = UserStatus.Online;
            switch (latencyAfter)
            {
                case var i when i > 150 && i < 500:
                    newStatus = UserStatus.AFK;
                    break;

                case var i when i >= 500:
                    newStatus = UserStatus.DoNotDisturb;
                    break;
            }
            if (!Equals(currentStatus, newStatus))
                await _client.SetStatusAsync(newStatus);
        }
    }
}