using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GeneralBot.Commands;
using GeneralBot.Models;
using Microsoft.Extensions.DependencyInjection;
using GeneralBot.Services;

namespace GeneralBot
{
    public class GeneralBot
    {
        private DiscordSocketClient _client;
        private IServiceProvider _services;
        private static void Main() => new GeneralBot().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            _client = new DiscordSocketClient();
            _services = await Initialize.StartAsync(_client);
            var config = _services.GetRequiredService<ConfigModel>();
            var logger = _services.GetRequiredService<LogService>();
            await _services.GetRequiredService<CommandHandler>().InitAsync();
            await _client.LoginAsync(TokenType.Bot, config.Token);
            await _client.StartAsync();
            await Task.Delay(-1);
        }
    }
}