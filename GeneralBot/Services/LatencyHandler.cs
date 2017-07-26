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