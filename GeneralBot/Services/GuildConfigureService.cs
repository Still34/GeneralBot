using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using GeneralBot.Databases.Context;

namespace GeneralBot.Services
{
    internal class GuildConfigureService
    {
        private readonly CoreContext _coreSettings;

        public GuildConfigureService(DiscordSocketClient client, CoreContext coreSettings)
        {
            client.GuildAvailable += GuildRegistration;
            _coreSettings = coreSettings;
        }

        private async Task GuildRegistration(SocketGuild guild)
        {
            var dbEntry = _coreSettings.GuildsSettings.SingleOrDefault(x => x.GuildId == guild.Id);
            if (dbEntry != null) return;
            // TODO: impl logging for guild registration
            await _coreSettings.GuildsSettings.AddAsync(new GuildSettings {GuildId = guild.Id});
            await _coreSettings.SaveChangesAsync();
        }
    }
}