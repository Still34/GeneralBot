using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using GeneralBot.Databases.Context;
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
    public class ConfigModule : ModuleBase<CustomCommandContext>
    {

        [Command("prefix")]
        [Summary("Changes the command prefix.")]
        public async Task<RuntimeResult> ConfigPrefix(string prefix)
        {
            var dbEntry = Context.CoreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id) ?? new GuildSettings();
            dbEntry.Prefix = prefix;
            Context.CoreSettings.Update(dbEntry);
            await Context.CoreSettings.SaveChangesAsync();
            return CommandRuntimeResult.FromSuccess($"Successfully changed prefix to {Format.Bold(prefix)}.");
        }

        [Command("mod-perms")]
        [Alias("mp")]
        [Summary("Changes the required permission to use mod commands.")]
        public async Task<RuntimeResult> ModeratorPermsSet(
            [OverrideTypeReader(typeof(GuildPermissionTypeReader))] [Remainder] GuildPermission guildPermission)
        {
            var dbEntry = Context.CoreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == Context.Guild.Id) ?? new GuildSettings();
            dbEntry.ModeratorPermission = guildPermission;
            Context.CoreSettings.Update(dbEntry);
            await Context.CoreSettings.SaveChangesAsync();
            return CommandRuntimeResult.FromSuccess($"Successfully changed the required moderator permission to {Format.Bold(guildPermission.Humanize(LetterCasing.Title))}.");
        }

        [Command("test")]
        public async Task Test()
        {
            Context.Logging.Log("test", LogSeverity.Info);
        }
    }
}