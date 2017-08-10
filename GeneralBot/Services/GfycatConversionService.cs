using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GeneralBot.Extensions;
using GeneralBot.Models.Config;
using GeneralBot.Models.Database.CoreSettings;
using Gfycat;

namespace GeneralBot.Services
{
    public class GfycatConversionService
    {
        private readonly ICoreRepository _coreSettings;
        private readonly GfycatClient _gfycatClient;
        private readonly LoggingService _loggingService;

        public GfycatConversionService(ConfigModel config, LoggingService loggingService,
            DiscordSocketClient discordClient, ICoreRepository coreSettings)
        {
            if (string.IsNullOrWhiteSpace(config.Credentials.Gfycat.ClientId) ||
                string.IsNullOrWhiteSpace(config.Credentials.Gfycat.Secret))
                throw new ArgumentNullException(nameof(config));
            _loggingService = loggingService;
            _coreSettings = coreSettings;
            discordClient.MessageReceived += FileUploadHandlerAsync;
            _gfycatClient = new GfycatClient(config.Credentials.Gfycat.ClientId, config.Credentials.Gfycat.Secret);
        }

        private async Task FileUploadHandlerAsync(SocketMessage msgArg)
        {
            // Checks if it's a user message.
            var msg = msgArg as SocketUserMessage;
            if (msg == null) return;
            if (msg.Author.IsBot) return;
            // Checks if it's a guild message.
            var channel = msg.Channel as SocketGuildChannel;
            if (channel == null) return;
            // Checks if the guild has specified for conversion.
            var dbEntry = await _coreSettings.GetOrCreateGuildSettingsAsync(channel.Guild);
            if (!dbEntry.IsGfyCatEnabled) return;
            // Begins attachment search.
            var attachments = msg.Attachments
                .Where(att => att.Filename.EndsWith(".mov", StringComparison.OrdinalIgnoreCase) ||
                              att.Filename.EndsWith(".webm", StringComparison.OrdinalIgnoreCase)).ToList();
            if (attachments.Count == 0) return;
            foreach (var attachment in attachments)
            {
                var _ = PerformGfyConversionAsync(attachment.Url, msg);
            }
        }

        private async Task PerformGfyConversionAsync(string url, IMessage msg)
        {
            using (msg.Channel.EnterTypingState())
            {
                await _loggingService.LogAsync($"Begins file conversion for {msg.Author} in {msg.GetPostedAt()}." +
                                               Environment.NewLine +
                                               $"Input: {url}", LogSeverity.Info).ConfigureAwait(false);
                var gfy = await _gfycatClient.CreateGfyAsync(url).ConfigureAwait(false);
                var completeGfy = await gfy.GetGfyWhenCompleteAsync().ConfigureAwait(false);
                await msg.Channel
                    .SendMessageAsync($"{msg.Author.Mention} {completeGfy.Url}\n" +
                                      "Here's the preview of the video that you just posted!")
                    .ConfigureAwait(false);
                await _loggingService.LogAsync($"Ends file conversion for {msg.Author} in {msg.GetPostedAt()}." +
                                               Environment.NewLine +
                                               $"Result: {completeGfy.Url}", LogSeverity.Info).ConfigureAwait(false);
            }
        }
    }
}