using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
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
        public async Task Cowsay([Remainder] string text)
        {
            using (var http = new HttpClient())
            {
                var output = await http.GetAsync($"http://cowsay.morecode.org/say?message={text.Replace(" ", "+")}&format=text");
                await ReplyAsync($"```{await output.Content.ReadAsStringAsync()}```");
            }
        }
    }
}