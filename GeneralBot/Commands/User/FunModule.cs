using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using GeneralBot.Extensions;
using GeneralBot.Models;
using GeneralBot.Results;

namespace GeneralBot.Commands.User
{
    [Summary("Fun Commands")]
    [Remarks("Want to goof around? Try these!")]
    public class FunModule : ModuleBase<SocketCommandContext>
    {
        public Random Random { get; set; }
        public ConfigModel Config { get; set; }

        [Command("8ball")]
        [Summary("Ask it any questions!")]
        public async Task<RuntimeResult> EightBall([Remainder] string input)
        {
            int responseCount = Config.Commands.EightBall.Responses.Count;
            if (responseCount == 0)
                // This should hopefully never happen.
                return CommandRuntimeResult.FromError("The 8ball responses are not yet set, contact the bot developers.");
            string response = Config.Commands.EightBall.Responses[Random.Next(0, responseCount)];
            var embed = new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder
                    {
                        Name = "The Magic 8Ball",
                        IconUrl = Context.Client.CurrentUser.GetAvatarUrlOrDefault()
                    },
                    Color = InputHelper.GetRandomColor(),
                    ThumbnailUrl = Config.Commands.EightBall.Image
                }
                .AddField("You asked...", input)
                .AddField("The 8 ball says...", response);
            await ReplyAsync("", embed: embed);
            return CommandRuntimeResult.FromSuccess();
        }

        [Command("choose")]
        [Alias("decide")]
        [Summary("Can't decide? Ask the bot to choose it for you!")]
        public async Task<RuntimeResult> Choose([Remainder] string input)
        {
            var regexParsed = Regex.Split(input, "or|;|,|and", RegexOptions.IgnoreCase);
            if (regexParsed.Length == 0)
                return CommandRuntimeResult.FromError("You need to supply more than one option!");
            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = "I think you should choose...",
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrlOrDefault()
                },
                Color = InputHelper.GetRandomColor(),
                Description = regexParsed[Random.Next(0, regexParsed.Length)]
            };
            await ReplyAsync("", embed: embed);
            return CommandRuntimeResult.FromSuccess();
        }
    }
}
