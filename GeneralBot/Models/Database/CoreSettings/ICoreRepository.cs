using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;

namespace GeneralBot.Models.Database.CoreSettings
{
    public interface ICoreRepository
    {
        Task UnregisterGuildAsync(IGuild guildId);
        Task SaveRepositoryAsync();

        string GetCommandPrefix(IGuild guild);

        IEnumerable<ActivityLogging> GetAllActivitySettings();
        ActivityLogging GetActivitySettings(IGuild guild);
        Task AddActivitySettingsAsync(ActivityLogging activitySettings);
        Task RemoveActivitySettingsAsync(ActivityLogging activitySettings);
        Task<ActivityLogging> GetOrCreateActivityAsync(IGuild guild);

        IEnumerable<GreetingSettings> GetAllGreetingSettings();
        GreetingSettings GetGreetingSettings(IGuild guild);
        Task AddGreetingSettingsAsync(GreetingSettings greetingSettings);
        Task RemoveGreetingsSettingsAsync(GreetingSettings greetingSettings);
        Task<GreetingSettings> GetOrCreateGreetingsAsync(IGuild guild);

        IEnumerable<GuildSettings> GetAllGuildSettings();
        GuildSettings GetGuildSettings(IGuild guild);
        Task AddGuildSettingsAsync(GuildSettings greetingSettings);
        Task RemoveGuildSettingsAsync(GuildSettings greetingSettings);
        Task<GuildSettings> GetOrCreateGuildSettingsAsync(IGuild guild);
    }
}