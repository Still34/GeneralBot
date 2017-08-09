using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using GeneralBot.Commands.Results;
using GeneralBot.Extensions;
using GeneralBot.Extensions.Helpers;
using GeneralBot.Preconditions;

namespace GeneralBot.Commands.Owners
{
    [Group("bot")]
    [Summary("Bot-specific Settings")]
    [Remarks("Bot settings for owners.")]
    [RequireOwners]
    public class BotModule : ModuleBase<SocketCommandContext>
    {
        private IDisposable _typing;
        public HttpClient HttpClient { get; set; }
        public InteractiveService InteractiveService { get; set; }

        protected override void BeforeExecute(CommandInfo command)
        {
            base.BeforeExecute(command);
            _typing = Context.Channel.EnterTypingState();
        }

        protected override void AfterExecute(CommandInfo command)
        {
            base.AfterExecute(command);
            _typing.Dispose();
        }

        [Command("username")]
        [Summary("Changes the bot's username.")]
        public async Task<RuntimeResult> ConfigUsernameAsync([Remainder] string username)
        {
            await Context.Client.CurrentUser.ModifyAsync(x => { x.Username = username; });
            return CommandRuntimeResult.FromSuccess($"Successfully changed username to {Format.Bold(username)}.");
        }

        [Command("luminance")]
        public async Task<RuntimeResult> GetLuminanceAsync(byte r, byte g, byte b)
        {
            var color = new Color(r, g, b);
            var embed = new EmbedBuilder
            {
                Color = color,
                Description = $"Luminance value = {color.GetLuminanceFromColor()}"
            };
            await ReplyAsync("", embed: embed);
            return CommandRuntimeResult.FromSuccess();
        }

        [Command("game")]
        [Summary("Changes the bot's playing status.")]
        public async Task<RuntimeResult> ConfigGameAsync([Remainder] string game)
        {
            await Context.Client.SetGameAsync(game);
            return CommandRuntimeResult.FromSuccess($"Successfully changed game to {Format.Bold(game)}.");
        }

        [Command("avatar")]
        [Summary("Changes the bot's avatar.")]
        public async Task<RuntimeResult> AvatarConfigureAsync()
        {
            await ReplyAsync("", embed: EmbedHelper.FromInfo(description: "Please upload the new avatar."));
            var message = await InteractiveService.NextMessageAsync(Context,
                new EnsureFromUserCriterion(Context.User.Id), TimeSpan.FromMinutes(5));
            Uri imageUri = null;
            if (!string.IsNullOrEmpty(message.Content))
            {
                var image = await WebHelper.GetImageUriAsync(HttpClient, message.Content);
                if (image != null) imageUri = image;
            }
            var attachment = message.Attachments.FirstOrDefault();
            if (attachment?.Height != null) Uri.TryCreate(attachment.Url, UriKind.RelativeOrAbsolute, out imageUri);
            if (imageUri == null) return CommandRuntimeResult.FromError("No valid images were detected.");
            var imageStream = await WebHelper.GetFileStreamAsync(HttpClient, imageUri);
            try
            {
                // ReSharper disable once AccessToDisposedClosure
                await Context.Client.CurrentUser.ModifyAsync(x => x.Avatar = new Image(imageStream));
            }
            finally
            {
                imageStream.Dispose();
            }
            return CommandRuntimeResult.FromSuccess("Successfully changed avatar.");
        }
    }
}