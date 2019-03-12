using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;

namespace GeneralBot.Models.Database.UserSettings
{
    public interface IUserRepository
    {
        Task SaveRepositoryAsync();

        Games GetGame(IUser user);
        Task<Games> GetOrCreateGameAsync(IUser user);
        IEnumerable<Games> GetAllGames();

        Task AddReminderAsync(Reminder reminder);
        IEnumerable<Reminder> GetReminders(IUser user);
        IEnumerable<Reminder> GetAllReminders();
        Task RemoveReminderAsync(Reminder reminder);
        Task RemoveRemindersAsync(IUser user);

        Task<Coordinate> AddOrUpdateCoordinatesAsync(IUser user, double longitude, double latitude);
        Coordinate GetCoordinates(IUser user);
        IEnumerable<Coordinate> GetAllCoordinates();
        Task RemoveCoordinatesAsync(IUser user);

        Profile GetProfile(IUser user);
        Task<Profile> GetOrCreateProfileAsync(IUser user);
        IEnumerable<Profile> GetProfiles();
    }
}