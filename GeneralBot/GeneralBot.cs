using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GeneralBot.Models.Config;
using Microsoft.Extensions.DependencyInjection;

namespace GeneralBot
{
    public class GeneralBot
    {
        private static void Main() => new GeneralBot().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            var client = new DiscordSocketClient();
            var services = await Initialize.StartAsync(client);
            var config = services.GetRequiredService<ConfigModel>();
            await client.LoginAsync(TokenType.Bot, config.Credentials.Discord);
            await client.StartAsync();
            await Task.Delay(-1);
        }
    }
}