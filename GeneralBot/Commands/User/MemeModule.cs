using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using GeneralBot.Results;

namespace GeneralBot.Commands.User
{
    public class MemeModule : ModuleBase<SocketCommandContext>
    {
        [Command("expand")]
        public Task<RuntimeResult> ExpandMeme([Remainder] string input)
        {
            var sb = new StringBuilder();
            foreach (char c in input)
            {
                sb.Append(c);
                sb.Append(" ");
            }
            return Task.FromResult<RuntimeResult>(CommandRuntimeResult.FromInfo(sb.ToString()));
        }
    }
}