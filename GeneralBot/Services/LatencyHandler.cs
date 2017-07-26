using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace GeneralBot.Services
{
    public class LatencyUpdatedHandler
    {
        private readonly DiscordSocketClient _client;

        public LatencyUpdatedHandler(DiscordSocketClient client)
        {
            _client = client;
            _client.LatencyUpdated += LatencyUpdatedAsync;
        }

        private async Task LatencyUpdatedAsync(int latencyBefore, int latencyAfter)
        {
            switch (latencyAfter)
            {
                case var i when i <= 150:
                    await _client.SetStatusAsync(UserStatus.Online);
                    break;

                case var i when i > 150 && i < 500:
                    await _client.SetStatusAsync(UserStatus.AFK);
                    break;

                case var i when i >= 500:
                    await _client.SetStatusAsync(UserStatus.DoNotDisturb);
                    break;
            }
        }
    }
}