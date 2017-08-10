using System;
using System.Globalization;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GeneralBot.Extensions;
using GeneralBot.Models.Database.CoreSettings;

namespace GeneralBot.Services
{
    /// <summary>
    ///     Configures guild registration entries in the database and welcome events.
    /// </summary>
    internal class ConfigureGuildService
    {
        private readonly ICoreRepository _coreSettings;
        private readonly LoggingService _loggingService;

        public ConfigureGuildService(DiscordSocketClient client, ICoreRepository coreSettings,
            LoggingService loggingService)
        {
            client.GuildAvailable += RegisterGuildAsync;
            client.JoinedGuild += RegisterGuildAsync;
            client.LeftGuild += UnregisterGuildAsync;
            client.UserJoined += WelcomeAsync;
            _coreSettings = coreSettings;
            _loggingService = loggingService;
        }

        private async Task RegisterGuildAsync(SocketGuild guild) =>
            await _coreSettings.GetOrCreateGuildSettingsAsync(guild);

        private async Task UnregisterGuildAsync(SocketGuild guild) =>
            await _coreSettings.UnregisterGuildAsync(guild);

        private async Task WelcomeAsync(SocketGuildUser user)
        {
            var guild = user.Guild;
            await _loggingService.LogAsync($"{user.GetFullnameOrDefault()} ({user.Id}) joined {guild} ({guild.Id}).",
                LogSeverity.Verbose);

            var dbEntry = await _coreSettings.GetOrCreateGreetingsAsync(guild);
            if (!dbEntry.IsJoinEnabled) return;
            var channel = guild.GetTextChannel(dbEntry.ChannelId);
            if (channel == null) return;
            string formattedMessage = dbEntry.WelcomeMessage
                .Replace("{mention}", user.Mention)
                .Replace("{username}", user.Username)
                .Replace("{discrim}", user.Discriminator)
                .Replace("{guild}", guild.Name)
                .Replace("{date}", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            var _ = channel.SendMessageAsync(formattedMessage);
        }
    }
}