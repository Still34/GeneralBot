using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeneralBot.Models.Database.CoreSettings
{
    public interface ICoreRepository
    {
        Task<ActivityLogging> GetOrCreateActivityAsync(ulong guildId);
        IEnumerable<ActivityLogging> GetAllActivitySettings();
        IEnumerable<GreetingSettings> GetAllGreetingSettings();
        IEnumerable<GuildSettings> GetAllGuildSettings();
        Task<GreetingSettings> GetOrCreateGreetingsAsync(ulong guildId);
        Task<GuildSettings> GetOrCreateGuildSettingsAsync(ulong guildId);
    }
}