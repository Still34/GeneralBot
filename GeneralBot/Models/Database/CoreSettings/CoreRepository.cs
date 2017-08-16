using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using GeneralBot.Services;

namespace GeneralBot.Models.Database.CoreSettings
{
    public class CoreRepository : ICoreRepository
    {
        private readonly CoreContext _coreContext;
        private readonly LoggingService _logging;

        public CoreRepository(CoreContext coreContext, LoggingService logging)
        {
            _coreContext = coreContext;
            _logging = logging;
        }

        public Task RemoveGreetingsSettingsAsync(GreetingSettings greetingSettings)
        {
            _coreContext.GreetingsSettings.Remove(greetingSettings);
            return SaveRepositoryAsync();
        }

        public async Task<GreetingSettings> GetOrCreateGreetingsAsync(IGuild guild)
        {
            var record = _coreContext.GreetingsSettings.SingleOrDefault(x => x.GuildId == guild.Id);
            if (record == null)
            {
                await _logging.LogAsync($"Registering greetings settings for {guild.Name} ({guild.Id})...",
                    LogSeverity.Info).ConfigureAwait(false);
                record = new GreetingSettings {GuildId = guild.Id};
                _coreContext.GreetingsSettings.Add(record);
                await SaveRepositoryAsync().ConfigureAwait(false);
            }
            return record;
        }

        public IEnumerable<GreetingSettings> GetAllGreetingSettings() => _coreContext.GreetingsSettings;

        public GreetingSettings GetGreetingSettings(IGuild guild) => _coreContext.GreetingsSettings.SingleOrDefault(
            x => x.GuildId == guild.Id);

        public async Task AddGreetingSettingsAsync(GreetingSettings greetingSettings)
        {
            await _coreContext.AddAsync(greetingSettings).ConfigureAwait(false);
            await SaveRepositoryAsync().ConfigureAwait(false);
        }

        public Task RemoveActivitySettingsAsync(ActivityLogging activitySettings)
        {
            _coreContext.ActivityLogging.Remove(activitySettings);
            return SaveRepositoryAsync();
        }

        public async Task<ActivityLogging> GetOrCreateActivityAsync(IGuild guild)
        {
            var record = _coreContext.ActivityLogging.SingleOrDefault(x => x.GuildId == guild.Id);
            if (record == null)
            {
                await _logging.LogAsync($"Registering logging settings for {guild.Name} ({guild.Id})...",
                    LogSeverity.Info).ConfigureAwait(false);
                record = new ActivityLogging {GuildId = guild.Id};
                _coreContext.ActivityLogging.Add(record);
                await SaveRepositoryAsync().ConfigureAwait(false);
            }
            return record;
        }

        public Task UnregisterGuildAsync(IGuild guild)
        {
            var guildRecords = _coreContext.GuildsSettings.Where(x => x.GuildId == guild.Id);
            if (guildRecords != null) _coreContext.RemoveRange(guildRecords);
            var activityRecords = _coreContext.ActivityLogging.Where(x => x.GuildId == guild.Id);
            if (activityRecords != null) _coreContext.RemoveRange(activityRecords);
            var greetingSettings = _coreContext.GreetingsSettings.Where(x => x.GuildId == guild.Id);
            if (greetingSettings != null) _coreContext.RemoveRange(greetingSettings);
            return SaveRepositoryAsync();
        }

        public Task SaveRepositoryAsync() => _coreContext.SaveChangesAsync();

        public string GetCommandPrefix(IGuild guild)
        {
            return _coreContext.GuildsSettings.SingleOrDefault(x => x.GuildId == guild.Id)?.CommandPrefix;
        }

        public IEnumerable<ActivityLogging> GetAllActivitySettings() => _coreContext.ActivityLogging;

        public ActivityLogging GetActivitySettings(IGuild guild) => _coreContext.ActivityLogging.SingleOrDefault(
            x => x.GuildId == guild.Id);

        public async Task AddActivitySettingsAsync(ActivityLogging activitySettings)
        {
            await _coreContext.ActivityLogging.AddAsync(activitySettings).ConfigureAwait(false);
            await SaveRepositoryAsync().ConfigureAwait(false);
        }

        public Task RemoveGuildSettingsAsync(GuildSettings greetingSettings)
        {
            _coreContext.GuildsSettings.Remove(greetingSettings);
            return SaveRepositoryAsync();
        }

        public async Task<GuildSettings> GetOrCreateGuildSettingsAsync(IGuild guild)
        {
            var record = _coreContext.GuildsSettings.SingleOrDefault(x => x.GuildId == guild.Id);
            if (record == null)
            {
                await _logging.LogAsync($"Registering guild settings for {guild.Name} ({guild.Id})...",
                    LogSeverity.Info).ConfigureAwait(false);
                record = new GuildSettings {GuildId = guild.Id};
                _coreContext.GuildsSettings.Add(record);
                await SaveRepositoryAsync().ConfigureAwait(false);
            }
            return record;
        }

        public IEnumerable<GuildSettings> GetAllGuildSettings() => _coreContext.GuildsSettings;

        public GuildSettings GetGuildSettings(IGuild guild) => _coreContext.GuildsSettings.SingleOrDefault(
            x => x.GuildId == guild.Id);

        public async Task AddGuildSettingsAsync(GuildSettings greetingSettings)
        {
            await _coreContext.GuildsSettings.AddAsync(greetingSettings).ConfigureAwait(false);
            await SaveRepositoryAsync().ConfigureAwait(false);
        }
    }
}