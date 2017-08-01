using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GeneralBot.Extensions.Helpers;
using GeneralBot.Models.Context;
using GeneralBot.Results;
using GeneralBot.Services;

namespace GeneralBot.Commands
{
    internal class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly CoreContext _coreSettings;
        private readonly LoggingService _loggingService;
        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services, DiscordSocketClient client, CommandService commandService,
            CoreContext settings, LoggingService loggingService)
        {
            _services = services;
            _client = client;
            _commandService = commandService;
            _coreSettings = settings;
            _loggingService = loggingService;
            _client.MessageReceived += CommandHandleAsync;
            _commandService.CommandExecuted += OnCommandExecutedAsync;
        }

        private async Task OnCommandExecutedAsync(CommandInfo commandInfo, ICommandContext context, IResult result)
        {
            if (string.IsNullOrEmpty(result.ErrorReason) || result.Error == CommandError.UnknownCommand) return;
            var embed = new EmbedBuilder();
            var severity = LogSeverity.Debug;
            switch (result)
            {
                case CommandRuntimeResult customResult:
                    switch (customResult.Type)
                    {
                        case ResultType.Unknown:
                            break;
                        case ResultType.Info:
                            severity = LogSeverity.Info;
                            embed = EmbedHelper.FromInfo(description: customResult.Reason);
                            break;
                        case ResultType.Warning:
                            severity = LogSeverity.Warning;
                            embed = EmbedHelper.FromWarning(description: customResult.Reason);
                            break;
                        case ResultType.Error:
                            severity = LogSeverity.Error;
                            embed = EmbedHelper.FromError(description: customResult.Reason);
                            break;
                        case ResultType.Success:
                            severity = LogSeverity.Verbose;
                            embed = EmbedHelper.FromSuccess(description: customResult.Reason);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    severity = LogSeverity.Error;
                    embed = EmbedHelper.FromError(description: result.ErrorReason);
                    break;
            }
            await context.Channel.SendMessageAsync("", embed: embed);
            await _loggingService.Log(
                $"{context.User} executed {commandInfo.Name} in {(context.Guild == null ? context.Channel.Name : $"{context.Channel.Name}/{context.Guild.Name}")}\n" +
                $"Result: {result.ErrorReason} ({result.GetType()})",
                severity);
        }

        public async Task InitAsync()
        {
            // Load TypeReaders dynamically because I'm lazy.
            var typeReaders = Assembly.GetEntryAssembly().GetTypes()
                .Where(x => x.GetTypeInfo().BaseType == typeof(TypeReader));
            foreach (var typeReader in typeReaders)
            {
                var method = typeof(CommandService).GetMethods()
                    .FirstOrDefault(x => !x.ContainsGenericParameters & (x.Name == "AddTypeReader"));
                method.Invoke(_commandService, new[] {typeReader, Activator.CreateInstance(typeReader)});
            }

            // Adds all command modules.
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task CommandHandleAsync(SocketMessage msgArg)
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
                    prefix = dbEntry.CommandPrefix;
            }
            if (!msg.HasStringPrefix(prefix, ref argPos)) return;

            var context = new SocketCommandContext(_client, msg);
            await _commandService.ExecuteAsync(context, argPos, _services);
        }
    }
}