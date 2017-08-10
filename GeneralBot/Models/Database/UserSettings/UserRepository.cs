using System.Collections.Generic;
using System.Linq;
using GeneralBot.Models.Context;

namespace GeneralBot.Models.Database.UserSettings
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _userContext;

        public UserRepository(UserContext userContext) => _userContext = userContext;

        public IEnumerable<Games> GetGames => _userContext.Games;

        public Games GetGame(ulong userId) => _userContext.Games.SingleOrDefault(x => x.UserId == userId);

        public Profile GetProfile(ulong userId) => _userContext.Profiles.SingleOrDefault(x => x.UserId == userId);

        public IEnumerable<Profile> GetProfiles() => _userContext.Profiles;

        public IEnumerable<Reminder> GetReminders(ulong userId) =>
            _userContext.Reminders.Where(x => x.UserId == userId);

        public IEnumerable<Reminder> GetAllReminders() => _userContext.Reminders;

        public Coordinate GetCoordinateAsync(ulong userId) =>
            _userContext.Coordinates.SingleOrDefault(x => x.UserId == userId);

        public IEnumerable<Coordinate> GetAllCoordinates() => _userContext.Coordinates;
    }
}