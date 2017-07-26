using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using GeneralBot.Databases.Context;
using GeneralBot.Results;

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
            return CommandRuntimeResult.FromSuccess($"Successfully changed prefix to {prefix}.");
        }
    }
}