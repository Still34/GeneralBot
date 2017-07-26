using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using GeneralBot.Databases.Context;
using GeneralBot.Preconditions;
using GeneralBot.Results;
using GeneralBot.Typereaders;
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

        [Command("prefix")]
        [Summary("Changes the command prefix.")]
        public async Task<RuntimeResult> ConfigPrefix(string prefix)
        {
            var dbEntry = CoreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id) ?? new GuildSettings();
            dbEntry.Prefix = prefix;
            CoreSettings.Update(dbEntry);
            await CoreSettings.SaveChangesAsync();
            return CommandRuntimeResult.FromSuccess($"Successfully changed prefix to {Format.Bold(prefix)}.");
        }

        [Command("mod-perms")]
        [Alias("mp")]
        public async Task<RuntimeResult> ModeratorPermsSet(
            [OverrideTypeReader(typeof(GuildPermissionTypeReader))] [Remainder] GuildPermission guildPermission)
        {
            var dbEntry = CoreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id) ?? new GuildSettings();
            dbEntry.ModeratorPermission = guildPermission;
            CoreSettings.Update(dbEntry);
            await CoreSettings.SaveChangesAsync();
            return CommandRuntimeResult.FromSuccess($"Successfully changed the required moderator permission to {Format.Bold(guildPermission.Humanize(LetterCasing.Title))}.");
        }
    }

    [Group("bot")]
    [Summary("Bot-specific Settings")]
    [Remarks("Bot settings for owners.")]
    [RequireOwners]
    public class BotModule : ModuleBase<SocketCommandContext>
    {
        [Command("username")]
        [Summary("Changes the bot's username.")]
        public async Task<RuntimeResult> ConfigUsername([Remainder] string username)
        {
            await Context.Client.CurrentUser.ModifyAsync(x => { x.Username = username; });
            return CommandRuntimeResult.FromSuccess($"Successfully changed username to {Format.Bold(username)}.");
        }

        [Command("game")]
        [Summary("Changes the bot's playing status.")]
        public async Task<RuntimeResult> ConfigGame([Remainder] string game)
        {
            await Context.Client.SetGameAsync(game);
            return CommandRuntimeResult.FromSuccess($"Successfully changed game to {Format.Bold(game)}.");
        }

        [Command("status")]
        [Summary("Changes the bot's status.")]
        public async Task<RuntimeResult> ConfigStatus(string status)
        {
            switch (status.ToLower())
            {
                case "online":
                    await Context.Client.SetStatusAsync(UserStatus.Online);
                    return CommandRuntimeResult.FromSuccess($"Successfully changed status to {Format.Bold("Online")}.");
                case "offline":
                    await Context.Client.SetStatusAsync(UserStatus.Offline);
                    return CommandRuntimeResult.FromSuccess($"Successfully changed status to {Format.Bold("Offline")}.");
                case "afk":
                    await Context.Client.SetStatusAsync(UserStatus.AFK);
                    return CommandRuntimeResult.FromSuccess($"Successfully changed status to {Format.Bold("AFK")}.");
                case "idle":
                    await Context.Client.SetStatusAsync(UserStatus.Idle);
                    return CommandRuntimeResult.FromSuccess($"Successfully changed status to {Format.Bold("Idle")}.");
                case "dnd":
                    await Context.Client.SetStatusAsync(UserStatus.DoNotDisturb);
                    return CommandRuntimeResult.FromSuccess($"Successfully changed status to {Format.Bold("Do Not Disturb")}.");
                case "invisible":
                    await Context.Client.SetStatusAsync(UserStatus.Invisible);
                    return CommandRuntimeResult.FromSuccess($"Successfully changed status to {Format.Bold("Invisible")}.");
                default:
                    return CommandRuntimeResult.FromError($"{Format.Bold(status)} is not a valid status.");
            }
        }
    }
}