using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
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
            return Task.FromResult<RuntimeResult>(CommandRuntimeResult.FromInfo(sb));
        }

        [Command("cowsay")]
        [Summary("Mooo!")]
        public async Task<RuntimeResult> Cowsay([Remainder] string text)
        {
            string parsedInput = WebUtility.HtmlEncode(text);
            using (var client = new HttpClient())
            using (var response = await client.GetAsync($"http://cowsay.morecode.org/say?message={parsedInput}&format=text"))
            {
                string output = await response.Content.ReadAsStringAsync();
                await ReplyAsync(Format.Code(output));
                return CommandRuntimeResult.FromSuccess();
            }
        }
    }
}