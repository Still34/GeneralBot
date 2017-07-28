using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using GeneralBot.Results;

namespace GeneralBot.Commands.User
{
    public class MemeModule : ModuleBase<SocketCommandContext>
    {
        [Command("expand")]
        [Summary("Replies with a s t h e t i c texts.")]
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
                if (!response.IsSuccessStatusCode) return CommandRuntimeResult.FromError("I cannot reach cowsay at the moment, please try again later!");
                string output = await response.Content.ReadAsStringAsync();
                // This should likely never happen, but just in case.
                if (string.IsNullOrEmpty(output)) return CommandRuntimeResult.FromError("Cowsay is out of reach, please try again with another text!");
                await ReplyAsync(output);
                return CommandRuntimeResult.FromSuccess();
            }
        }
    }
}