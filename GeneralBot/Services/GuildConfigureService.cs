using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GeneralBot.Databases.Context;
using GeneralBot.Extensions;

namespace GeneralBot.Services
{
    internal class GuildConfigureService
    {
        private readonly CoreContext _coreSettings;
        private readonly LoggingService _loggingService;

        public GuildConfigureService(DiscordSocketClient client, CoreContext coreSettings, LoggingService loggingService)
        {
            client.GuildAvailable += RegisterGuild;
            client.JoinedGuild += RegisterGuild;
            client.LeftGuild += UnregisterGuild;
            client.UserJoined += WelcomeMember;
            _coreSettings = coreSettings;
            _loggingService = loggingService;
        }

        private async Task UnregisterGuild(SocketGuild guild)
        {
            var dbEntry = _coreSettings.GuildsSettings.Where(x => x.GuildId == guild.Id);
            if (dbEntry == null) return;
            await _loggingService.Log($"Left {guild}, unregistering...", LogSeverity.Info);
            _coreSettings.GuildsSettings.RemoveRange(dbEntry);
            await _coreSettings.SaveChangesAsync();
        }

        private async Task RegisterGuild(SocketGuild guild) => await GetOrRegisterGuildEntry(guild);

        private async Task<GuildSettings> GetOrRegisterGuildEntry(SocketGuild guild)
        {
            var dbEntry = _coreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == guild.Id);
            if (dbEntry != null) return dbEntry;

            await _loggingService.Log($"New guild {guild} found, registering...", LogSeverity.Info);
            dbEntry = new GuildSettings {GuildId = guild.Id};
            await _coreSettings.GuildsSettings.AddAsync(dbEntry);
            await _coreSettings.SaveChangesAsync();
            return dbEntry;
        }

        private async Task WelcomeMember(SocketGuildUser user)
        {
            var guild = user.Guild;
            await _loggingService.Log($"{user.GetFullnameOrDefault()} ({user.Id}) joined {guild} ({guild.Id}).", LogSeverity.Verbose);

            var dbEntry = await GetOrRegisterGuildEntry(guild);
            if (!dbEntry.WelcomeEnable) return;
            var channel = guild.GetTextChannel(dbEntry.WelcomeChannel);
            if (channel == null) return;
            string formattedMessage = dbEntry.WelcomeMessage.Replace("{mention}", user.Mention)
                .Replace("{username}", user.Username)
                .Replace("{discrim}", user.Discriminator)
                .Replace("{guild}", guild.Name)
                .Replace("{date}", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            var _ = channel.SendMessageAsync(formattedMessage);
        }
    }
}