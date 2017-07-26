using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using GeneralBot.Databases.Context;
using GeneralBot.Results;

namespace GeneralBot.Commands.Admin
{
    [Group("server")]
    [RequireContext(ContextType.Guild)]
    public class ConfigModule : ModuleBase<SocketCommandContext>
    {
        public CoreContext CoreSettings { get; set; }

        [Command("prefix")]
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
