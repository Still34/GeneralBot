using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GeneralBot.Models.Config;
using Microsoft.Extensions.DependencyInjection;

namespace GeneralBot
{
    /// <summary>
    /// Main entry point of the bot.
    /// </summary>
    public class GeneralBot
    {
        private static void Main() => new GeneralBot().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            // Creates a new instance of the Discord client.
            var client = new DiscordSocketClient();

            // Initializes all of thr required services.
            var services = await Initialize.StartAsync(client);

            // Gets the credentials required to start the bot.
            var config = services.GetRequiredService<ConfigModel>();

            // Begins Discord login.
            await client.LoginAsync(TokenType.Bot, config.Credentials.Discord);
            await client.StartAsync();
            await Task.Delay(-1);
        }
    }
}