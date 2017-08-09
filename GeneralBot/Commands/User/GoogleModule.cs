using System.Threading.Tasks;
using Discord.Commands;
using GeneralBot.Commands.Results;
using GeneralBot.Models.Config;
using GeneralBot.Services;

namespace GeneralBot.Commands.User
{
    [Summary("Google Commands")]
    [Remarks("Want to search for some data? Use these!")]
    public class GoogleModule : ModuleBase<SocketCommandContext>
    {
        public GoogleService GoogleService { get; set; }

        [Command("google")]
        [Alias("search")]
        public async Task<RuntimeResult> SearchAsync([Remainder] string query)
        {
            await ReplyAsync("", embed: await GoogleService.SearchAsync(query));
            return CommandRuntimeResult.FromSuccess();
        }

    }
}