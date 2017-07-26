using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GeneralBot.Databases.Context;

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
    }
}