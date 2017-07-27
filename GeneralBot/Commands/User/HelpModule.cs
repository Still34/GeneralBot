﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GeneralBot.Databases.Context;
using GeneralBot.Extensions;
using GeneralBot.Results;

namespace GeneralBot.Commands.User
{
    [Group("help")]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        public CoreContext CoreSettings { get; set; }
        public CommandService CommandService { get; set; }
        public IServiceProvider ServiceProvider { get; set; }

        [Command]
        public async Task<RuntimeResult> HelpSpecificCommand(string input)
        {
            var commandInfos = await GetCommandInfosAsync(input);
            if (commandInfos.Count == 0)
                return CommandRuntimeResult.FromError("I could not find any related commands!");

            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = $"Here are some commands related to \"{input}\"...",
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrlOrDefault()
                }
            };
            foreach (var commandInfo in commandInfos)
            {
                if (embed.Fields.Count > 10)
                {
                    embed.AddInlineField($"And {commandInfos.Count - embed.Fields.Count} more...", "Refine your search term to see more!");
                    break;
                }
                embed.AddInlineField($"{GetCommandPrefix(Context.Guild)}{BuildCommandInfo(commandInfo)}", commandInfo.Summary);
            }
            await ReplyAsync("", embed: embed);
            return CommandRuntimeResult.FromSuccess();
        }

        private string GetCommandPrefix(SocketGuild guild) => guild == null ? "!" : CoreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id).CommandPrefix;

        private static string BuildCommandInfo(CommandInfo cmdInfo) => $"{cmdInfo.Aliases.First()} {cmdInfo.Parameters.GetParamsUsage()}";

        private async Task<IReadOnlyCollection<CommandInfo>> GetCommandInfosAsync(string input)
        {
            var commandInfos = new List<CommandInfo>();
            foreach (var module in CommandService.Modules)
            {
                foreach (var command in module.Commands)
                {
                    var check = await command.CheckPreconditionsAsync(Context, ServiceProvider);
                    if (!check.IsSuccess) continue;
                    if (command.Aliases.Any(x => x.ContainsCaseInsensitive(input)) ||
                        module.IsSubmodule &&
                        module.Aliases.Any(x => x.ContainsCaseInsensitive(input)))
                        commandInfos.Add(command);
                }
            }
            return commandInfos;
        }
    }
}