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
        public CoreContext CoreSettings { get; set; }
        public HttpClient HttpClient { get; set; }
        public InteractiveService InteractiveService { get; set; }

        [Command("icon")]
        [Summary("Changes the server icon.")]
        public async Task<RuntimeResult> ChangeGuildIconAsync(Uri url = null)
        {
            if (url == null)
            {
                await ReplyAsync("", embed: EmbedHelper.FromInfo("New icon", "Please upload your new server icon."));
                var response = await InteractiveService.NextMessageAsync(Context, timeout: TimeSpan.FromMinutes(5));
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
            using (var image = await WebHelper.GetFileStreamAsync(HttpClient, url))
            {
                // ReSharper disable once AccessToDisposedClosure
                await Context.Guild.ModifyAsync(x => x.Icon = new Image(image));
            }
            return CommandRuntimeResult.FromSuccess("The server icon has been changed!");
        }

        [Command("prefix")]
        [Summary("Changes the command prefix.")]
        public async Task<RuntimeResult> ConfigPrefixAsync(string prefix)
        {
            var dbEntry = CoreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id);

            dbEntry.CommandPrefix = prefix;
            CoreSettings.Update(dbEntry);
            await CoreSettings.SaveChangesAsync();
            return CommandRuntimeResult.FromSuccess($"Successfully changed prefix to {Format.Bold(prefix)}.");
        }

        [Command("mod-perms")]
        [Alias("mp")]
        [Summary("Changes the required permission to use mod commands.")]
        public async Task<RuntimeResult> ModeratorPermsSetAsync([Remainder] GuildPermission guildPermission)
        {
            var dbEntry = CoreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id);

            dbEntry.ModeratorPermission = guildPermission;
            CoreSettings.Update(dbEntry);
            await CoreSettings.SaveChangesAsync();
            return CommandRuntimeResult.FromSuccess(
                $"Successfully changed the required moderator permission to {Format.Bold(guildPermission.Humanize(LetterCasing.Title))}.");
        }

        [Command("invite")]
        [Alias("inv")]
        [Summary("Change server invite command settings.")]
        public async Task<RuntimeResult> ToggleInviteAsync(bool? shouldEnable = null)
        {
            var dbEntry = CoreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id);

            string result;
            switch (shouldEnable ?? !dbEntry.IsInviteAllowed)
            {
                case true:
                    dbEntry.IsInviteAllowed = true;
                    result = "Invite has been **enabled**.";
                    break;
                default:
                    dbEntry.IsInviteAllowed = false;
                    result = "Invite has been **disabled**.";
                    break;
            }
            CoreSettings.Update(dbEntry);
            await CoreSettings.SaveChangesAsync();
            return CommandRuntimeResult.FromSuccess(result);
        }

        [Command("gfycat")]
        [Alias("convert", "gfy")]
        [Summary("Change gfycat conversion settings.")]
        public async Task<RuntimeResult> ToggleGfycatAsync(bool? shouldEnable = null)
        {
            var dbEntry = CoreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id);

            string result;
            switch (shouldEnable ?? !dbEntry.IsGfyCatEnabled)
            {
                case true:
                    dbEntry.IsGfyCatEnabled = true;
                    result = "Auto gfycat conversion has been **enabled**.";
                    break;
                default:
                    dbEntry.IsGfyCatEnabled = false;
                    result = "Auto gfycat conversion has been **disabled**.";
                    break;
            }
            CoreSettings.Update(dbEntry);
            await CoreSettings.SaveChangesAsync();
            return CommandRuntimeResult.FromSuccess(result);
        }

        [Group("log")]
        [Alias("logging")]
        public class Log : ModuleBase<SocketCommandContext>
        {
            public CoreContext CoreSettings { get; set; }

            [Command("voice")]
            [Alias("voicechat", "vc")]
            public async Task<RuntimeResult> ToggleVcLoggingAsync(bool? shouldEnable = null)
            {
                var dbEntry = CoreSettings.ActivityLogging.SingleOrDefault(x => x.GuildId == Context.Guild.Id);

                string result;
                switch (shouldEnable ?? !dbEntry.ShouldLogVoice)
                {
                    case true:
                        dbEntry.ShouldLogVoice = true;
                        result = "Voice activity logging has been **enabled**.";
                        break;
                    default:
                        dbEntry.ShouldLogVoice = false;
                        result = "Voice activity logging has been **disabled**.";
                        break;
                }
                CoreSettings.Update(dbEntry);
                await CoreSettings.SaveChangesAsync();
                return CommandRuntimeResult.FromSuccess(result);
            }

            [Command("join")]
            [Alias("userjoin", "userjoined", "joined")]
            public async Task<RuntimeResult> ToggleJoinLoggingAsync(bool? shouldEnable = null)
            {
                var dbEntry = CoreSettings.ActivityLogging.SingleOrDefault(x => x.GuildId == Context.Guild.Id);

                string result;
                switch (shouldEnable ?? !dbEntry.ShouldLogJoin)
                {
                    case true:
                        dbEntry.ShouldLogJoin = true;
                        result = "User join logging has been **enabled**.";
                        break;
                    default:
                        dbEntry.ShouldLogJoin = false;
                        result = "User join logging has been **disabled**.";
                        break;
                }
                CoreSettings.Update(dbEntry);
                await CoreSettings.SaveChangesAsync();
                return CommandRuntimeResult.FromSuccess(result);
            }

            [Command("leave")]
            [Alias("left", "userleft", "userleave")]
            public async Task<RuntimeResult> ToggleLeaveLoggingAsync(bool? shouldEnable = null)
            {
                var dbEntry = CoreSettings.ActivityLogging.SingleOrDefault(x => x.GuildId == Context.Guild.Id);

                string result;
                switch (shouldEnable ?? !dbEntry.ShouldLogLeave)
                {
                    case true:
                        dbEntry.ShouldLogLeave = true;
                        result = "User leave logging has been **enabled**.";
                        break;
                    default:
                        dbEntry.ShouldLogLeave = false;
                        result = "User leave logging has been **disabled**.";
                        break;
                }
                CoreSettings.Update(dbEntry);
                await CoreSettings.SaveChangesAsync();
                return CommandRuntimeResult.FromSuccess(result);
            }
        }

        [Group("welcome")]
        [Alias("w", "greet")]
        [Summary("Configures the welcome setting.")]
        public class WelcomeModule : ModuleBase<SocketCommandContext>
        {
            public ConfigModel Config { get; set; }
            public CoreContext CoreSettings { get; set; }

            [Command]
            [Summary("Checks the current status of the welcome feature.")]
            public async Task WelcomeAsync()
            {
                var dbEntry = CoreSettings.GreetingsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id);
                string formattedMessage = dbEntry.WelcomeMessage.Replace("{mention}", Context.User.Mention)
                    .Replace("{username}", Context.User.Username)
                    .Replace("{discrim}", Context.User.Discriminator)
                    .Replace("{guild}", Context.Guild.Name)
                    .Replace("{date}", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
                var em = new EmbedBuilder
                    {
                        Title = "Current welcome configuration:",
                        Description = $"Currently {(dbEntry.IsJoinEnabled ? "Enabled" : "Disabled")}",
                        Color = new Color(52, 152, 219),
                        ThumbnailUrl = Config.Icons.Announce
                    }
                    .AddInlineField("Current message:", dbEntry.WelcomeMessage)
                    .AddInlineField("Example:", formattedMessage);
                await ReplyAsync("", embed: em);
            }

            [Command("enable")]
            [Alias("e")]
            [Summary("Enables the welcome setting on the current guild.")]
            public async Task<RuntimeResult> EnableWelcomeAsync()
            {
                var greetingSettings =
                    CoreSettings.GreetingsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id);

                var guildSettings = CoreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id);
                if (greetingSettings.IsJoinEnabled)
                    return CommandRuntimeResult.FromError("The welcome message is already enabled!");
                greetingSettings.IsJoinEnabled = true;
                CoreSettings.Update(greetingSettings);
                await CoreSettings.SaveChangesAsync();
                return CommandRuntimeResult.FromSuccess(
                    $"Successfully enabled the welcome message! If you haven't configure the welcome message by using `{guildSettings.CommandPrefix}server welcome message`");
            }

            [Command("disable")]
            [Alias("d")]
            [Summary("Disables the welcome setting on the current guild.")]
            public async Task<RuntimeResult> DisableWelcomeAsync()
            {
                var dbEntry = CoreSettings.GreetingsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id);
                if (!dbEntry.IsJoinEnabled)
                    return CommandRuntimeResult.FromError("The welcome message is already disabled!");
                dbEntry.IsJoinEnabled = false;
                CoreSettings.Update(dbEntry);
                await CoreSettings.SaveChangesAsync();
                return CommandRuntimeResult.FromSuccess("Successfully disabled the welcome message!");
            }

            [Command("message")]
            [Alias("m")]
            [Summary("Changes the welcome message on the current guild.")]
            [Remarks("Placeholders: {mention}, {username}, {discrim}, {guild}, {date}")]
            public async Task<RuntimeResult> ConfigMessageAsync([Remainder] string message)
            {
                var dbEntry = CoreSettings.GreetingsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id);
                if (message.Length > 1024) return CommandRuntimeResult.FromError("Your welcome message is too long!");
                dbEntry.WelcomeMessage = message;
                CoreSettings.Update(dbEntry);
                await CoreSettings.SaveChangesAsync();
                return CommandRuntimeResult.FromSuccess($"Welcome message set to: {Format.Bold(message)}");
            }

            [Command("channel")]
            [Alias("c")]
            [Summary("Changes the welcome channel on the current guild.")]
            public async Task<RuntimeResult> ConfigChannelAsync(SocketTextChannel channel)
            {
                var dbEntry = CoreSettings.GreetingsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id);
                dbEntry.ChannelId = channel.Id;
                CoreSettings.Update(dbEntry);
                await CoreSettings.SaveChangesAsync();
                return CommandRuntimeResult.FromSuccess($"Welcome channel set to: {channel.Mention}");
            }
        }
    }
}