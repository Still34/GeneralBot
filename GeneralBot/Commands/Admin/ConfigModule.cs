using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using GeneralBot.Commands.Results;
using GeneralBot.Extensions.Helpers;
using GeneralBot.Models.Config;
using GeneralBot.Models.Database.CoreSettings;
using Humanizer;

namespace GeneralBot.Commands.Admin
{
    [Group("server")]
    [Summary("Server-specific Settings")]
    [Remarks("Server settings for admins.")]
    [RequireContext(ContextType.Guild)]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class ConfigModule : ModuleBase<SocketCommandContext>
    {
        public ICoreRepository CoreRepository { get; set; }
        public HttpClient HttpClient { get; set; }
        public InteractiveService InteractiveService { get; set; }

        [Command("icon")]
        [Summary("Changes the server icon.")]
        public async Task<RuntimeResult> ChangeGuildIconAsync(Uri url = null)
        {
            if (url == null)
            {
                await ReplyAsync("", embed: EmbedHelper.FromInfo("New icon", "Please upload your new server icon.")).ConfigureAwait(false);
                var response = await InteractiveService.NextMessageAsync(Context, timeout: TimeSpan.FromMinutes(5)).ConfigureAwait(false);
                if (response == null)
                    return CommandRuntimeResult.FromError("You did not upload your picture in time.");
                if (!Uri.TryCreate(response.Content, UriKind.RelativeOrAbsolute, out url))
                {
                    var attachment = response.Attachments.FirstOrDefault();
                    if (attachment?.Height != null)
                        url = new Uri(response.Attachments.FirstOrDefault().Url);
                    else
                        return CommandRuntimeResult.FromError("You did not upload any valid pictures.");
                }
            }
            using (var image = await WebHelper.GetFileStreamAsync(HttpClient, url).ConfigureAwait(false))
            {
                // ReSharper disable once AccessToDisposedClosure
                await Context.Guild.ModifyAsync(x => x.Icon = new Image(image)).ConfigureAwait(false);
            }
            return CommandRuntimeResult.FromSuccess("The server icon has been changed!");
        }

        [Command("prefix")]
        [Summary("Changes the command prefix.")]
        public async Task<RuntimeResult> ConfigPrefixAsync(string prefix)
        {
            var record = await CoreRepository.GetOrCreateGuildSettingsAsync(Context.Guild).ConfigureAwait(false);
            record.CommandPrefix = prefix;
            await CoreRepository.SaveRepositoryAsync().ConfigureAwait(false);
            return CommandRuntimeResult.FromSuccess($"Successfully changed prefix to {Format.Bold(prefix)}.");
        }

        [Command("mod-perms")]
        [Alias("mp")]
        [Summary("Changes the required permission to use mod commands.")]
        public async Task<RuntimeResult> ModeratorPermsSetAsync([Remainder] GuildPermission guildPermission)
        {
            var record = await CoreRepository.GetOrCreateGuildSettingsAsync(Context.Guild).ConfigureAwait(false);
            record.ModeratorPermission = guildPermission;
            await CoreRepository.SaveRepositoryAsync().ConfigureAwait(false);
            return CommandRuntimeResult.FromSuccess(
                $"Successfully changed the required moderator permission to {Format.Bold(guildPermission.Humanize(LetterCasing.Title))}.");
        }

        [Command("invite")]
        [Alias("inv")]
        [Summary("Change server invite command settings.")]
        public async Task<RuntimeResult> ToggleInviteAsync(bool? shouldEnable = null)
        {
            var record = await CoreRepository.GetOrCreateGuildSettingsAsync(Context.Guild).ConfigureAwait(false);

            string result;
            switch (shouldEnable ?? !record.IsInviteAllowed)
            {
                case true:
                    record.IsInviteAllowed = true;
                    result = "Invite has been **enabled**.";
                    break;
                default:
                    record.IsInviteAllowed = false;
                    result = "Invite has been **disabled**.";
                    break;
            }

            await CoreRepository.SaveRepositoryAsync().ConfigureAwait(false);
            return CommandRuntimeResult.FromSuccess(result);
        }

        [Command("gfycat")]
        [Alias("convert", "gfy")]
        [Summary("Change gfycat conversion settings.")]
        public async Task<RuntimeResult> ToggleGfycatAsync(bool? shouldEnable = null)
        {
            var record = await CoreRepository.GetOrCreateGuildSettingsAsync(Context.Guild).ConfigureAwait(false);
            string result;
            switch (shouldEnable ?? !record.IsGfyCatEnabled)
            {
                case true:
                    record.IsGfyCatEnabled = true;
                    result = "Auto gfycat conversion has been **enabled**.";
                    break;
                default:
                    record.IsGfyCatEnabled = false;
                    result = "Auto gfycat conversion has been **disabled**.";
                    break;
            }
            await CoreRepository.SaveRepositoryAsync().ConfigureAwait(false);
            return CommandRuntimeResult.FromSuccess(result);
        }

        [Group("log")]
        [Alias("logging")]
        public class Log : ModuleBase<SocketCommandContext>
        {
            public ICoreRepository CoreRepository { get; set; }

            [Command("channel")]
            public async Task<RuntimeResult> SetLogChannelAsync(SocketTextChannel channel = null)
            {
                var record = await CoreRepository.GetOrCreateActivityAsync(Context.Guild).ConfigureAwait(false);
                if (channel == null)
                {
                    var targetChannel = Context.Guild.GetTextChannel(record.LogChannel);
                    return targetChannel == null
                        ? CommandRuntimeResult.FromError("You have not set up a valid log channel yet!")
                        : CommandRuntimeResult.FromInfo($"The current log channel is: {Format.Bold(targetChannel.Name)}");
                }
                if (record.LogChannel == channel.Id)
                    return CommandRuntimeResult.FromInfo("The current log channel is already the specified channel!");
                record.LogChannel = channel.Id;
                await CoreRepository.SaveRepositoryAsync().ConfigureAwait(false);
                return CommandRuntimeResult.FromSuccess($"The current log channel has been set to {Format.Bold(channel.Name)}!");
            }

            [Command("voice")]
            [Alias("voicechat", "vc")]
            public async Task<RuntimeResult> ToggleVcLoggingAsync(bool? shouldEnable = null)
            {
                var record = await CoreRepository.GetOrCreateActivityAsync(Context.Guild).ConfigureAwait(false);
                string result;
                switch (shouldEnable ?? !record.ShouldLogVoice)
                {
                    case true:
                        record.ShouldLogVoice = true;
                        result = "Voice activity logging has been **enabled**.";
                        break;
                    default:
                        record.ShouldLogVoice = false;
                        result = "Voice activity logging has been **disabled**.";
                        break;
                }
                await CoreRepository.SaveRepositoryAsync().ConfigureAwait(false);
                return CommandRuntimeResult.FromSuccess(result);
            }

            [Command("join")]
            [Alias("userjoin", "userjoined", "joined")]
            public async Task<RuntimeResult> ToggleJoinLoggingAsync(bool? shouldEnable = null)
            {
                var record = await CoreRepository.GetOrCreateActivityAsync(Context.Guild).ConfigureAwait(false);
                string result;
                switch (shouldEnable ?? !record.ShouldLogJoin)
                {
                    case true:
                        record.ShouldLogJoin = true;
                        result = "User join logging has been **enabled**.";
                        break;
                    default:
                        record.ShouldLogJoin = false;
                        result = "User join logging has been **disabled**.";
                        break;
                }
                await CoreRepository.SaveRepositoryAsync().ConfigureAwait(false);
                return CommandRuntimeResult.FromSuccess(result);
            }

            [Command("leave")]
            [Alias("left", "userleft", "userleave")]
            public async Task<RuntimeResult> ToggleLeaveLoggingAsync(bool? shouldEnable = null)
            {
                var record = await CoreRepository.GetOrCreateActivityAsync(Context.Guild).ConfigureAwait(false);
                string result;
                switch (shouldEnable ?? !record.ShouldLogLeave)
                {
                    case true:
                        record.ShouldLogLeave = true;
                        result = "User leave logging has been **enabled**.";
                        break;
                    default:
                        record.ShouldLogLeave = false;
                        result = "User leave logging has been **disabled**.";
                        break;
                }

                await CoreRepository.SaveRepositoryAsync().ConfigureAwait(false);
                return CommandRuntimeResult.FromSuccess(result);
            }
        }

        [Group("welcome")]
        [Alias("w", "greet")]
        [Summary("Configures the welcome setting.")]
        public class WelcomeModule : ModuleBase<SocketCommandContext>
        {
            public ConfigModel Config { get; set; }
            public ICoreRepository CoreRepository { get; set; }

            [Command]
            [Summary("Checks the current status of the welcome feature.")]
            public async Task WelcomeAsync()
            {
                var record = await CoreRepository.GetOrCreateGreetingsAsync(Context.Guild).ConfigureAwait(false);
                string formattedMessage = record.WelcomeMessage.Replace("{mention}", Context.User.Mention)
                    .Replace("{username}", Context.User.Username)
                    .Replace("{discrim}", Context.User.Discriminator)
                    .Replace("{guild}", Context.Guild.Name)
                    .Replace("{date}", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
                var em = new EmbedBuilder
                    {
                        Title = "Current welcome configuration:",
                        Description = $"Currently {(record.IsJoinEnabled ? "Enabled" : "Disabled")}",
                        Color = new Color(52, 152, 219),
                        ThumbnailUrl = Config.Icons.Announce
                    }
                    .AddInlineField("Current message:", record.WelcomeMessage)
                    .AddInlineField("Example:", formattedMessage);
                await ReplyAsync("", embed: em).ConfigureAwait(false);
            }

            [Command("enable")]
            [Alias("e")]
            [Summary("Enables the welcome setting on the current guild.")]
            public async Task<RuntimeResult> EnableWelcomeAsync()
            {
                var greetingSettings = await CoreRepository.GetOrCreateGreetingsAsync(Context.Guild).ConfigureAwait(false);
                var guildSettings = await CoreRepository.GetOrCreateGuildSettingsAsync(Context.Guild).ConfigureAwait(false);
                if (greetingSettings.IsJoinEnabled)
                    return CommandRuntimeResult.FromError("The welcome message is already enabled!");
                greetingSettings.IsJoinEnabled = true;
                await CoreRepository.SaveRepositoryAsync().ConfigureAwait(false);
                return CommandRuntimeResult.FromSuccess(
                    $"Successfully enabled the welcome message! If you haven't configure the welcome message by using `{guildSettings.CommandPrefix}server welcome message`");
            }

            [Command("disable")]
            [Alias("d")]
            [Summary("Disables the welcome setting on the current guild.")]
            public async Task<RuntimeResult> DisableWelcomeAsync()
            {
                var record = await CoreRepository.GetOrCreateGreetingsAsync(Context.Guild).ConfigureAwait(false);
                if (!record.IsJoinEnabled)
                    return CommandRuntimeResult.FromError("The welcome message is already disabled!");
                record.IsJoinEnabled = false;
                await CoreRepository.SaveRepositoryAsync().ConfigureAwait(false);
                return CommandRuntimeResult.FromSuccess("Successfully disabled the welcome message!");
            }

            [Command("message")]
            [Alias("m")]
            [Summary("Changes the welcome message on the current guild.")]
            [Remarks("Placeholders: {mention}, {username}, {discrim}, {guild}, {date}")]
            public async Task<RuntimeResult> ConfigMessageAsync([Remainder] string message)
            {
                var record = await CoreRepository.GetOrCreateGreetingsAsync(Context.Guild).ConfigureAwait(false);
                if (message.Length > 1024) return CommandRuntimeResult.FromError("Your welcome message is too long!");
                record.WelcomeMessage = message;
                await CoreRepository.SaveRepositoryAsync().ConfigureAwait(false);
                return CommandRuntimeResult.FromSuccess($"Welcome message set to: {Format.Bold(message)}");
            }

            [Command("channel")]
            [Alias("c")]
            [Summary("Changes the welcome channel on the current guild.")]
            public async Task<RuntimeResult> ConfigChannelAsync(SocketTextChannel channel)
            {
                var record = await CoreRepository.GetOrCreateGreetingsAsync(Context.Guild).ConfigureAwait(false);
                record.ChannelId = channel.Id;
                await CoreRepository.SaveRepositoryAsync().ConfigureAwait(false);
                return CommandRuntimeResult.FromSuccess($"Welcome channel set to: {channel.Mention}");
            }
        }
    }
}