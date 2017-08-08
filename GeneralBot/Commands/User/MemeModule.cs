using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using GeneralBot.Commands.Results;
using GeneralBot.Extensions.Helpers;
using GeneralBot.Models.Config;
using GeneralBot.Models.Reddit;
using Newtonsoft.Json;
using ImageSharp;
using ImageSharp.Formats;

namespace GeneralBot.Commands.User
{
    [Summary("Meme Commands")]
    [Remarks("For your daily doses of memery, what more could I say?")]
    public class MemeModule : ModuleBase<SocketCommandContext>
    {
        public ConfigModel Config { get; set; }
        public HttpClient HttpClient { get; set; }
        public Random Random { get; set; }
        private IDisposable TypingDisposable { get; set; }

        protected override void BeforeExecute(CommandInfo command)
        {
            base.BeforeExecute(command);
            TypingDisposable = Context.Channel.EnterTypingState();
        }

        protected override void AfterExecute(CommandInfo command)
        {
            base.AfterExecute(command);
            TypingDisposable.Dispose();
        }

        [Command("expand")]
        [Summary("Replies with a s t h e t i c texts.")]
        public Task<RuntimeResult> ExpandMemeAsync([Remainder] string input)
        {
            var sb = new StringBuilder();
            foreach (char c in input)
            {
                if (c == ' ') continue;
                sb.Append(c);
                sb.Append(" ");
            }
            return Task.FromResult<RuntimeResult>(CommandRuntimeResult.FromInfo(sb));
        }

        [Command("cowsay")]
        [Summary("Moo!")]
        public async Task<RuntimeResult> CowsayAsync([Remainder] string text)
        {
            string parsedInput = WebUtility.HtmlEncode(text);
            using (var response =
                await HttpClient.GetAsync($"http://cowsay.morecode.org/say?message={parsedInput}&format=text"))
            {
                if (!response.IsSuccessStatusCode)
                {
                    return CommandRuntimeResult.FromError(
                        "I cannot reach cowsay at the moment, please try again later!");
                }
                string output = await response.Content.ReadAsStringAsync();
                // This should likely never happen, but just in case.
                if (string.IsNullOrEmpty(output))
                {
                    return CommandRuntimeResult.FromError(
                        "Cowsay is out of reach, please try again with another text!");
                }
                await ReplyAsync(output);
                return CommandRuntimeResult.FromSuccess();
            }
        }

        [Command("thinking")]
        [Alias("rthinking", "think", "🤔")]
        public async Task<RuntimeResult> ThinkingAsync()
        {
            using (var response = await HttpClient.GetAsync("https://www.reddit.com/r/Thinking/.json"))
            {
                if (!response.IsSuccessStatusCode)
                    return CommandRuntimeResult.FromError("Reddit is out of reach, please try again later!");

                var result =
                    JsonConvert.DeserializeObject<RedditResponseModel>(await response.Content.ReadAsStringAsync());
                var children = Context.Channel.IsNsfw
                    ? result.Data.Children
                    : result.Data.Children.Where(x => !x.Data.IsNsfw).ToList();
                int index = Random.Next(children.Count);
                var post = children[index];
                string title = Config.Commands.ThinkingTitles[Random.Next(Config.Commands.ThinkingTitles.Count)];
                var builder = new EmbedBuilder
                {
                    Title = post.Data.Title,
                    Description = $"{(post.Data.IsNsfw ? $"{title} (NSFW)" : title)}\n\nPosted by u/{post.Data.Author}",
                    Url = "https://www.reddit.com/" + post.Data.Permalink,
                    Color = ColorHelper.GetRandomColor(),
                    ThumbnailUrl = post.Data.Url,
                };
                await ReplyAsync("", embed: builder);
                return CommandRuntimeResult.FromSuccess();
            }
        }

        [Command("needsmorejpeg")]
        public async Task<RuntimeResult> NeedsMoreJpeg()
        {
            var message = (await Context.Channel.GetMessagesAsync().Flatten())?.FirstOrDefault(x => x.Attachments.Any());
            if (message == null)
                return CommandRuntimeResult.FromError("No images found!");
            var attachment = message.Attachments.FirstOrDefault() as Attachment;
            var image = ImageSharp.Image.Load(await WebHelper.GetFileStreamAsync(HttpClient, new Uri(attachment.Url)));
            using (var stream = new MemoryStream())
            {
                image.SaveAsJpeg(stream, new JpegEncoder { Quality = 2 });
                stream.Seek(0, SeekOrigin.Begin);
                await Context.Channel.SendFileAsync(stream, "needsmorejpeg.jpeg");
            }
            return CommandRuntimeResult.FromSuccess();
        }
    }
}