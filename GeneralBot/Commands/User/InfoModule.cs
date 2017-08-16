using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GeneralBot.Commands.Results;
using GeneralBot.Extensions;
using GeneralBot.Models.Config;
using GeneralBot.Models.Database.CoreSettings;
using Humanizer;

namespace GeneralBot.Commands.User
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        public ConfigModel Config { get; set; }
        public ICoreRepository CoreRepository { get; set; }

        [Command("invite")]
        [RequireContext(ContextType.Guild)]
        public async Task<RuntimeResult> GetOrCreateInviteAsync()
        {
            if (Context.Channel is SocketGuildChannel channel)
            {
                var record = await CoreRepository.GetOrCreateGuildSettingsAsync(Context.Guild).ConfigureAwait(false);
                if (!record.IsInviteAllowed)
                    return CommandRuntimeResult.FromError("The admin has disabled this command.");
                var invite = await channel.GetLastInviteAsync(true).ConfigureAwait(false);
                await ReplyAsync(invite.Url).ConfigureAwait(false);
            }
            return CommandRuntimeResult.FromSuccess();
        }

        [Command("info")]
        public async Task<RuntimeResult> GetInfoAsync()
        {
            var embedBuilder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = "Bot Info:",
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrlOrDefault()
                },
                ThumbnailUrl =
                    "https://emojipedia-us.s3.amazonaws.com/thumbs/120/twitter/103/information-source_2139.png",
                Color = new Color(61, 138, 192)
            };
            // Owners
            embedBuilder.AddField("Owners",
                string.Join(", ", Config.Owners.Select(x => Context.Client.GetUser(x).ToString())));

            // Application uptime
            var currentProcess = Process.GetCurrentProcess();
            embedBuilder.AddField("Uptime", (DateTime.Now - currentProcess.StartTime).Humanize());

            // Memory report
            var memInfoTitleBuilder = new StringBuilder();
            var memInfoDescriptionBuilder = new StringBuilder();
            var heapBytes = GC.GetTotalMemory(false).Bytes();
            memInfoTitleBuilder.Append("Heap Size");
            memInfoDescriptionBuilder.Append(
                $"{Math.Round(heapBytes.LargestWholeNumberValue, 2)} {heapBytes.LargestWholeNumberSymbol}");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var workingSetBytes = currentProcess.WorkingSet64.Bytes();
                memInfoTitleBuilder.Append(" / Working Set");
                memInfoDescriptionBuilder.Append(
                    $" / {Math.Round(workingSetBytes.LargestWholeNumberValue, 2)} {workingSetBytes.LargestWholeNumberSymbol}");
            }
            embedBuilder.AddInlineField(memInfoTitleBuilder.ToString(), memInfoDescriptionBuilder);

            // Application latency
            embedBuilder.AddInlineField("Latency", Context.Client.Latency + "ms");

            // Discord application creation date
            var appInfo = await Context.Client.GetApplicationInfoAsync().ConfigureAwait(false);
            embedBuilder.AddInlineField("Created On", appInfo.CreatedAt.UtcDateTime);

            // Last updated on based on file modification date
            embedBuilder.AddInlineField("Last Update",
                File.GetLastWriteTimeUtc(Assembly.GetEntryAssembly().Location));

            // Lib version
            embedBuilder.AddInlineField("Discord.NET Version", DiscordConfig.Version);
            await ReplyAsync("", embed: embedBuilder).ConfigureAwait(false);
            return CommandRuntimeResult.FromSuccess();
        }

        /// <summary>
        ///     Modified from https://gist.github.com/foxbot/d220afcbbcadaa7fc5521493846032a7
        /// </summary>
        [Command("latency")]
        [Alias("ping", "pong", "rtt")]
        [Summary("Returns the current estimated round-trip latency over WebSocket")]
        public async Task GetLatencyAsync()
        {
            ulong target;
            var cts = new CancellationTokenSource();

            Task WaitTarget(SocketMessage message)
            {
                if (message.Id != target) return Task.CompletedTask;
                cts.Cancel();
                return Task.CompletedTask;
            }

            int latency = Context.Client.Latency;
            var stopwatch = Stopwatch.StartNew();
            var pingMessage = await ReplyAsync($"heartbeat: {latency}ms, init: ---, rtt: ---").ConfigureAwait(false);
            long init = stopwatch.ElapsedMilliseconds;
            target = pingMessage.Id;
            stopwatch.Restart();
            Context.Client.MessageReceived += WaitTarget;

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30), cts.Token).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                long rtt = stopwatch.ElapsedMilliseconds;
                stopwatch.Stop();
                await pingMessage.ModifyAsync(x => x.Content = $"heartbeat: {latency}ms, init: {init}ms, rtt: {rtt}ms").ConfigureAwait(false);
                return;
            }
            finally
            {
                Context.Client.MessageReceived -= WaitTarget;
            }
            stopwatch.Stop();
            await pingMessage.ModifyAsync(x => x.Content = $"heartbeat: {latency}ms, init: {init}ms, rtt: timeout").ConfigureAwait(false);
        }

        [Group("help")]
        public class HelpModule : ModuleBase<SocketCommandContext>
        {
            public CommandService CommandService { get; set; }
            public ICoreRepository CoreSettings { get; set; }
            public IServiceProvider ServiceProvider { get; set; }

            [Command]
            [Summary("Need help for a specific command? Use this!")]
            public async Task<RuntimeResult> HelpSpecificCommandAsync(string input)
            {
                var commandInfos = await GetCommandInfosAsync(input).ConfigureAwait(false);
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
                    if (embed.Fields.Count > 5)
                    {
                        embed.AddInlineField($"And {commandInfos.Count - embed.Fields.Count} more...",
                            "Refine your search term to see more!");
                        break;
                    }
                    embed.AddField(x =>
                    {
                        x.Name = $"{GetCommandPrefixAsync(Context.Guild)}{BuildCommandInfo(commandInfo)}";
                        x.Value = commandInfo.Summary ?? "No summary.";
                    });
                }
                await ReplyAsync("", embed: embed).ConfigureAwait(false);
                return CommandRuntimeResult.FromSuccess();
            }

            private async Task<string> GetCommandPrefixAsync(SocketGuild guild) => guild == null
                ? "!"
                : (await CoreSettings.GetOrCreateGuildSettingsAsync(guild).ConfigureAwait(false))?.CommandPrefix;

            private static string BuildCommandInfo(CommandInfo cmdInfo) =>
                $"{cmdInfo.Aliases.First()} {cmdInfo.Parameters.GetParamsUsage()}";

            private async Task<IReadOnlyCollection<CommandInfo>> GetCommandInfosAsync(string input)
            {
                var commandInfos = new List<CommandInfo>();
                foreach (var module in CommandService.Modules)
                foreach (var command in module.Commands)
                {
                    var check = await command.CheckPreconditionsAsync(Context, ServiceProvider).ConfigureAwait(false);
                    if (!check.IsSuccess) continue;
                    if (command.Aliases.Any(x => x.ContainsCaseInsensitive(input)) ||
                        module.IsSubmodule &&
                        module.Aliases.Any(x => x.ContainsCaseInsensitive(input)))
                        commandInfos.Add(command);
                }
                return commandInfos;
            }
        }
    }
}