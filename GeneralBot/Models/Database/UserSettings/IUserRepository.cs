using System.Collections.Generic;
using GeneralBot.Models.Context;

namespace GeneralBot.Models.Database.UserSettings
{
    public interface IUserRepository
    {
        IEnumerable<Games> GetGames { get; }

        IEnumerable<Coordinate> GetAllCoordinates();
        IEnumerable<Reminder> GetAllReminders();
        Coordinate GetCoordinateAsync(ulong userId);
        Games GetGame(ulong userId);
        Profile GetProfile(ulong userId);
        IEnumerable<Profile> GetProfiles();
        IEnumerable<Reminder> GetReminders(ulong userId);
    }
}