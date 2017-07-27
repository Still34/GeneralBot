using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GeneralBot.Databases.Context;
using System;

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

        private async Task RegisterGuild(SocketGuild guild)
        {
            var dbEntry = _coreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == guild.Id);
            if (dbEntry != null) return;
            await _loggingService.Log($"New guild {guild} found, registering...", LogSeverity.Info);
            await _coreSettings.GuildsSettings.AddAsync(new GuildSettings {GuildId = guild.Id});
            await _coreSettings.SaveChangesAsync();
        }

        private async Task WelcomeMember(SocketGuildUser user)
        {
            var guild = user.Guild;
            var dbEntry = _coreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == guild.Id);
            if (dbEntry == null) await RegisterGuild(guild);
            if (!dbEntry.EnableWelcome) return;
            var channel = guild.GetChannel(dbEntry.WelcomeChannel) as SocketTextChannel;
            if (channel == null) return;
            var formattedMessage = dbEntry.WelcomeMessage.Replace("{mention}", user.Mention)
                .Replace("{username}", user.Username)
                .Replace("{discrim}", user.Discriminator)
                .Replace("{guild}", guild.Name)
                .Replace("{date}", DateTime.UtcNow.ToString());
            await channel.SendMessageAsync(formattedMessage);
        }
    }
}