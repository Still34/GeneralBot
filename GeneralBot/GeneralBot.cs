using System;
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
        // ReSharper disable once AsyncConverter.AsyncAwaitMayBeElidedHighlighting
        // ReSharper disable once AsyncConverter.ConfigureAwaitHighlighting
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once AsyncConverter.AsyncMethodNamingHighlighting
        //meowpuffygottem
        private static async Task Main() => await new GeneralBot().StartAsync();

        public async Task StartAsync()
        {
            try
            {
                // Creates a new instance of the Discord client.
                var client = new DiscordSocketClient();

                // Initializes all of the required services.
                var services = await Initialize.StartAsync(client).ConfigureAwait(false);

                // Gets the credentials required to start the bot.
                var config = services.GetRequiredService<ConfigModel>();

                // Begins Discord login.
                await client.LoginAsync(TokenType.Bot, config.Credentials.Discord).ConfigureAwait(false);
                await client.StartAsync().ConfigureAwait(false);
                await Task.Delay(-1).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
    }
}
