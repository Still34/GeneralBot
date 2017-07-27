using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using GeneralBot.Databases.Context;
using GeneralBot.Results;
using GeneralBot.Typereaders;
using Humanizer;
using Discord.WebSocket;

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

        [Command("prefix")]
        [Summary("Changes the command prefix.")]
        public async Task<RuntimeResult> ConfigPrefix(string prefix)
        {
            var dbEntry = CoreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id) ?? new GuildSettings();
            dbEntry.CommandPrefix = prefix;
            CoreSettings.Update(dbEntry);
            await CoreSettings.SaveChangesAsync();
            return CommandRuntimeResult.FromSuccess($"Successfully changed prefix to {Format.Bold(prefix)}.");
        }

        [Command("mod-perms")]
        [Alias("mp")]
        [Summary("Changes the required permission to use mod commands.")]
        public async Task<RuntimeResult> ModeratorPermsSet(
            [OverrideTypeReader(typeof(GuildPermissionTypeReader))] [Remainder] GuildPermission guildPermission)
        {
            var dbEntry = CoreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id) ?? new GuildSettings();
            dbEntry.ModeratorPermission = guildPermission;
            CoreSettings.Update(dbEntry);
            await CoreSettings.SaveChangesAsync();
            return CommandRuntimeResult.FromSuccess($"Successfully changed the required moderator permission to {Format.Bold(guildPermission.Humanize(LetterCasing.Title))}.");
        }

        [Group("welcome")]
        [Alias("w")]
        [Summary("Configures the welcome setting.")]
        public class WelcomeModule : ModuleBase<SocketCommandContext>
        {
            public CoreContext CoreSettings { get; set; }

            [Command("")]
            [Summary("Checks the current status of the welcome feature.")]
            public async Task<RuntimeResult> Welcome()
            {
                var dbEntry = CoreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id) ?? new GuildSettings();
                return CommandRuntimeResult.FromInfo($"The current welcome message is {Format.Bold(dbEntry.WelcomeMessage)} (Enabled: {dbEntry.WelcomeEnable})");
            }

            [Command("enable")]
            [Alias("e")]
            [Summary("Enables the welcome setting on the current guild.")]
            public async Task<RuntimeResult> EnableWelcome()
            {
                var dbEntry = CoreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id) ?? new GuildSettings();
                if (dbEntry.WelcomeEnable) return CommandRuntimeResult.FromError("The welcome message is already enabled!");
                dbEntry.WelcomeEnable = true;
                CoreSettings.Update(dbEntry);
                await CoreSettings.SaveChangesAsync();
                return CommandRuntimeResult.FromSuccess($"Successfully enabled the welcome message! If you haven't configure the welcome message by using `{dbEntry.CommandPrefix}server welcome message`");
            }

            [Command("disable")]
            [Alias("d")]
            [Summary("Disables the welcome setting on the current guild.")]
            public async Task<RuntimeResult> DisableWelcome()
            {
                var dbEntry = CoreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id) ?? new GuildSettings();
                if (!dbEntry.WelcomeEnable) return CommandRuntimeResult.FromError("The welcome message is already disabled!");
                dbEntry.WelcomeEnable = false;
                CoreSettings.Update(dbEntry);
                await CoreSettings.SaveChangesAsync();
                return CommandRuntimeResult.FromSuccess($"Successfully disabled the welcome message!");
            }

            [Command("message")]
            [Alias("m")]
            [Summary("Changes the welcome message on the current guild.")]
            public async Task<RuntimeResult> ConfigMessage(string message)
            {
                var dbEntry = CoreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id) ?? new GuildSettings();
                dbEntry.WelcomeMessage = message;
                CoreSettings.Update(dbEntry);
                await CoreSettings.SaveChangesAsync();
                return CommandRuntimeResult.FromSuccess($"Welcome message set to: {Format.Bold(message)}");
            }

            [Command("channel")]
            [Alias("c")]
            [Summary("Changes the welcome channel on the current guild.")]
            public async Task<RuntimeResult> ConfigChannel(SocketTextChannel channel)
            {
                var dbEntry = CoreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id) ?? new GuildSettings();
                dbEntry.WelcomeChannel = channel.Id;
                CoreSettings.Update(dbEntry);
                await CoreSettings.SaveChangesAsync();
                return CommandRuntimeResult.FromSuccess($"Welcome channel set to: {channel.Mention}");
            }
        }
    }
}