using System.Threading.Tasks;
using Discord.Commands;
using GeneralBot.Commands.Results;
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
            var embedQuery = await GoogleService.SearchAsync(query).ConfigureAwait(false);
            if (embedQuery == null)
                return CommandRuntimeResult.FromError("I could not get the search result for now, try again later.");
            await ReplyAsync("", embed: embedQuery).ConfigureAwait(false);
            return CommandRuntimeResult.FromSuccess();
        }
    }
}