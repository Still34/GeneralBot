using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralBot.Models.Database.CoreSettings
{
    public class CoreRepository : ICoreRepository
    {
        private readonly CoreContext _coreContext;

        public CoreRepository(CoreContext coreContext) => _coreContext = coreContext;

        public async Task<GreetingSettings> GetOrCreateGreetingsAsync(ulong guildId)
        {
            var record = _coreContext.GreetingsSettings.SingleOrDefault(x => x.GuildId == guildId);
            if (record == null)
            {
                record = new GreetingSettings {GuildId = guildId};
                _coreContext.GreetingsSettings.Add(record);
                await _coreContext.SaveChangesAsync();
            }
            return record;
        }

        public IEnumerable<GreetingSettings> GetAllGreetingSettings() => _coreContext.GreetingsSettings;

        public async Task<ActivityLogging> GetOrCreateActivityAsync(ulong guildId)
        {
            var record = _coreContext.ActivityLogging.SingleOrDefault(x => x.GuildId == guildId);
            if (record == null)
            {
                record = new ActivityLogging {GuildId = guildId};
                _coreContext.ActivityLogging.Add(record);
                await _coreContext.SaveChangesAsync();
            }
            return record;
        }

        public IEnumerable<ActivityLogging> GetAllActivitySettings() => _coreContext.ActivityLogging;

        public async Task<GuildSettings> GetOrCreateGuildSettingsAsync(ulong guildId)
        {
            var record = _coreContext.GuildsSettings.SingleOrDefault(x => x.GuildId == guildId);
            if (record == null)
            {
                record = new GuildSettings {GuildId = guildId};
                _coreContext.GuildsSettings.Add(record);
                await _coreContext.SaveChangesAsync();
            }
            return record;
        }

        public IEnumerable<GuildSettings> GetAllGuildSettings() => _coreContext.GuildsSettings;
    }
}