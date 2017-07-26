using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GeneralBot.Databases.Context;
using GeneralBot.Results;

namespace GeneralBot.Commands
{
    internal class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly CoreContext _coreSettings;
        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services, DiscordSocketClient client, CommandService commandService, CoreContext settings)
        {
            _services = services;
            _client = client;
            _commandService = commandService;
            _coreSettings = settings;
            _client.MessageReceived += CommandHandling;
            _commandService.CommandExecuted += OnCommandExecuted;
        }

        private static async Task OnCommandExecuted(CommandInfo commandInfo, ICommandContext context, IResult result)
        {
            if (result is CommandRuntimeResult customResult)
            {
                var embed = new EmbedBuilder();
                switch (customResult.Type)
                {
                    case ResultType.Unknown:
                        break;
                    case ResultType.Info:
                        break;
                    case ResultType.Warning:
                        break;
                    case ResultType.Error:
                        break;
                    case ResultType.Success:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                await context.Channel.SendMessageAsync("", embed: embed);
            }
        }

        public async Task InitAsync() => await _commandService.AddModulesAsync(Assembly.GetEntryAssembly());

        private async Task CommandHandling(SocketMessage msgArg)
        {
            // Bail when it's not an user message.
            var msg = msgArg as SocketUserMessage;
            if (msg == null) return;
            // Also bail if it's a bot message to avoid "certain" situations.
            if (msg.Author.IsBot) return;

            int argPos = 0;
            string prefix = "!";
            // Checks if the channel is a guild channel, if so, attempts to fetch prefix
            if (msg.Channel is SocketGuildChannel guildChannel)
            {
                var dbEntry = _coreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == guildChannel.Guild.Id);
                if (dbEntry != null)
                    prefix = dbEntry.Prefix;
            }
            if (!msg.HasStringPrefix(prefix, ref argPos)) return;

            var context = new SocketCommandContext(_client, msg);
            var result = await _commandService.ExecuteAsync(context, argPos, _services);
        }
    }
}