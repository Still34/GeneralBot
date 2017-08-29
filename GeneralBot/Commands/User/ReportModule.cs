using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using GeneralBot.Commands.Results;
using GeneralBot.Extensions;
using GeneralBot.Extensions.Helpers;
using GeneralBot.Models.Config;
using GeneralBot.Models.Database.CoreSettings;
using Humanizer;

namespace GeneralBot.Commands.User
{
    [Group("report")]
    [RequireContext(ContextType.DM)]
    public class ReportModule : ModuleBase<SocketCommandContext>
    {
        private const string EmbedTitle = "Incident Report";
        private const string ReportChannelName = "report-logs";
        private readonly Regex _numberRegex = new Regex("[0-9]");
        private readonly TimeSpan _responseTimeout = TimeSpan.FromMinutes(5);
        private IDisposable _typing;
        public ConfigModel Config { get; set; }
        public CoreRepository CoreSettings { get; set; }
        public InteractiveService InteractiveService { get; set; }

        protected override void BeforeExecute(CommandInfo command)
        {
            base.BeforeExecute(command);
            _typing = Context.Channel.EnterTypingState();
        }

        protected override void AfterExecute(CommandInfo command)
        {
            base.AfterExecute(command);
            _typing.Dispose();
        }

        [Command]
        public async Task<RuntimeResult> ReportIncidentAsync()
        {
            // #1, query guild from user.
            var guilds = Context.Client.Guilds.Where(guild => guild.Users.Any(user => user.Id == Context.User.Id))
                .ToList();
            var guildBuilder = new StringBuilder("Which guild would you like to report to? (Choose by number)\n\n");
            int guildCount = 0;
            foreach (var guild in guilds)
            {
                guildBuilder.AppendLine($"{guildCount}. {guild.Name}");
                guildCount++;
            }
            await ReplyAsync("",
                embed: EmbedHelper.FromInfo(EmbedTitle, guildBuilder.ToString().Truncate(2000)).Build()).ConfigureAwait(false);
            var selectionMessage = await InteractiveService.NextMessageAsync(Context, timeout: _responseTimeout).ConfigureAwait(false);
            if (selectionMessage == null)
                return CommandRuntimeResult.FromError($"You did not reply in {_responseTimeout.Humanize()}.");
            if (!int.TryParse(_numberRegex.Match(selectionMessage.Content)?.Value, out int selection) ||
                selection > guilds.Count)
                return CommandRuntimeResult.FromError("You did not pick a valid selection.");

            // #2, query report from user.
            // TODO: Allow users to upload more than one image.
            await ReplyAsync("",
                embed: EmbedHelper.FromInfo(EmbedTitle,
                    "Please describe the incident in details, preferably with evidence.").Build()).ConfigureAwait(false);
            var reportMessage = await InteractiveService.NextMessageAsync(Context, timeout: _responseTimeout).ConfigureAwait(false);
            if (reportMessage == null)
                return CommandRuntimeResult.FromError($"You did not reply in {_responseTimeout.Humanize()}.");
            string reportContent = reportMessage.Content;
            if (reportContent.Length > 2048)
                return CommandRuntimeResult.FromError("Your report was too long. Please start over.");

            // #2.1, confirm report from user.
            var reportEmbed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = $"A new report has been filed by {Context.User}...",
                    IconUrl = Context.User.GetAvatarUrlOrDefault()
                },
                Description = reportContent,
                ThumbnailUrl = Config.Icons.Warning,
                Color = Color.Red
            };
            if (reportMessage.Attachments.Any())
                reportEmbed.AddField("Attachments", string.Join(", ", reportMessage.Attachments.Select(x => x.Url)));
            reportEmbed.AddField("Report time", DateTimeOffset.UtcNow);
            await ReplyAsync("Please review your report. Enter Y to confirm.", embed: reportEmbed.Build()).ConfigureAwait(false);
            var confirmMessage = await InteractiveService.NextMessageAsync(Context, timeout: _responseTimeout).ConfigureAwait(false);
            if (confirmMessage == null)
                return CommandRuntimeResult.FromError($"You did not reply in {_responseTimeout.Humanize()}.");
            if (!confirmMessage.Content.ToLower().Contains("y"))
                return CommandRuntimeResult.FromInfo("Dropping report...");

            // #3, send the report.
            var reportChannel = await GetReportChannelAsync(guilds[selection]).ConfigureAwait(false);
            var reportRoles = await GetModeratorRolesAsync(guilds[selection]).ConfigureAwait(false);
            await reportChannel.SendMessageAsync(
                string.Join(", ", reportRoles.Select(x => x.Mention)) ?? "New report has been filed.",
                embed: reportEmbed.Build()).ConfigureAwait(false);

            return CommandRuntimeResult.FromSuccess("Your report has been sent.");
        }

        private async Task<ITextChannel> GetReportChannelAsync(SocketGuild guild)
        {
            // Attempts to get text channel from database entry.
            var record = await CoreSettings.GetOrCreateGuildSettingsAsync(guild).ConfigureAwait(false);
            var reportChannel = guild.GetTextChannel(record.ReportChannel);
            if (reportChannel != null) return reportChannel;

            // Attempts to get text channel by name.
            var searchChannel = guild.TextChannels.FirstOrDefault(x => x.Name.ToLower().Contains(ReportChannelName));
            if (searchChannel != null)
            {
                record.ReportChannel = searchChannel.Id;
                await CoreSettings.SaveRepositoryAsync().ConfigureAwait(false);
                return searchChannel;
            }

            // Attempts to create a new text channel.
            var newChannel = await guild.CreateTextChannelAsync(ReportChannelName).ConfigureAwait(false);
            await newChannel.AddPermissionOverwriteAsync(guild.EveryoneRole,
                new OverwritePermissions(readMessages: PermValue.Deny)).ConfigureAwait(false);
            var modRoles = await GetModeratorRolesAsync(guild).ConfigureAwait(false);
            foreach (var modRole in modRoles)
            {
                await newChannel.AddPermissionOverwriteAsync(modRole,
                    new OverwritePermissions(readMessages: PermValue.Allow)).ConfigureAwait(false);
            }
            return newChannel;
        }

        private async Task<IEnumerable<SocketRole>> GetModeratorRolesAsync(SocketGuild guild)
        {
            var record = await CoreSettings.GetOrCreateGuildSettingsAsync(guild).ConfigureAwait(false);
            return guild.Roles.Where(x => x.Permissions.Has(record.ModeratorPermission));
        }
    }
}