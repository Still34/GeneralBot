using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GeneralBot.Extensions.Helpers;
using GeneralBot.Models.Config;
using GeneralBot.Models.Database.CoreSettings;
using Gfycat;

namespace GeneralBot.Services
{
    public class GfycatConversionService
    {
        private readonly CoreContext _coreSettings;
        private readonly GfycatClient _gfycatClient;
        private readonly HttpClient _httpClient;
        private readonly LoggingService _loggingService;

        public GfycatConversionService(ConfigModel config, LoggingService loggingService,
            DiscordSocketClient discordClient, HttpClient httpClient, CoreContext coreSettings)
        {
            if (string.IsNullOrWhiteSpace(config.Credentials.Gfycat.ClientId) ||
                string.IsNullOrWhiteSpace(config.Credentials.Gfycat.Secret))
                throw new ArgumentNullException(nameof(config));
            _loggingService = loggingService;
            _httpClient = httpClient;
            _coreSettings = coreSettings;
            discordClient.MessageReceived += FileUploadHandlerAsync;
            _gfycatClient = new GfycatClient(config.Credentials.Gfycat.ClientId, config.Credentials.Gfycat.Secret);
        }

        private Task FileUploadHandlerAsync(SocketMessage msgArg)
        {
            // Checks if it's a user message.
            var msg = msgArg as SocketUserMessage;
            if (msg == null) return Task.CompletedTask;
            if (msg.Author.IsBot) return Task.CompletedTask;
            // Checks if it's a guild message.
            var channel = msg.Channel as SocketGuildChannel;
            if (channel == null) return Task.CompletedTask;
            // Checks if the guild has specified for conversion.
            var dbEntry = _coreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == channel.Guild.Id);
            if (dbEntry == null || !dbEntry.IsGfyCatEnabled) return Task.CompletedTask;
            // Begins attachment search.
            var attachments =
                msg.Attachments.Where(att => att.Filename.EndsWith(".mov") || att.Filename.EndsWith(".webm")).ToList();
            if (attachments.Count == 0) return Task.CompletedTask;
            foreach (var attachment in attachments)
            {
                var _ = Task.Run(() => PerformGfyConversionAsync(attachment.Url, msg));
            }
            return Task.CompletedTask;
        }

        private async Task PerformGfyConversionAsync(string url, IMessage msg)
        {
            using (var stream = await WebHelper.GetFileStreamAsync(_httpClient, new Uri(url)))
            {
                await _loggingService.LogAsync($"Begins file conversion for {url} for {msg.Author}.",
                    LogSeverity.Info).ConfigureAwait(false);
                var gfy = await _gfycatClient.CreateGfyAsync(stream);
                var completeGfy = await gfy.GetGfyWhenCompleteAsync();
                await msg.Channel.SendMessageAsync(
                    $"Hey {msg.Author.Mention}! I've reuploaded the file for you!\n\n{completeGfy.Url}");
                await _loggingService.LogAsync($"Ends file conversion for {url} for {msg.Author}.",
                    LogSeverity.Info).ConfigureAwait(false);
            }
        }
    }
}